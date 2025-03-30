using Audit.Http;
using Audit.Core;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Serilog;
using Polly;
using Polly.Extensions.Http;
using Fuse8.BackendInternship.PublicApi.ModelBinders;
using Fuse8.BackendInternship.PublicApi.JsonConverters;
using Fuse8.BackendInternship.PublicApi.Middlewares;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;

namespace Fuse8.BackendInternship.PublicApi;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers(options =>
		{
			// Добавляем глобальный фильтр для обработки исключений
			options.Filters.Add<CurrencyExceptionFilter>();
		})
		// Добавляем глобальные настройки для преобразования Json
		.AddJsonOptions(
		options =>
		{
			// Добавляем конвертер для енама
			// По умолчанию енам преобразуется в цифровое значение
			// Этим конвертером задаем перевод в строковое значение
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

        services.AddOptions<CurrencyHttpApiSettings>()
            .Bind(_configuration.GetSection("CurrencyHttpApi"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

		services.AddOptions<CurrencySettigns>()
			.Bind(_configuration.GetSection("Currency"))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.AddHttpClient<CurrencyService>()
			.AddPolicyHandler(HttpPolicyExtensions
			// Настраиваем повторный запрос при получении ошибок сервера (HTTP-код = 5XX) и для таймаута выполнения запроса (HTTP-код = 408)
				.HandleTransientHttpError()
				.WaitAndRetryAsync(
					retryCount: 3,
					sleepDurationProvider: retryAttempt =>
					{
					// Настраиваем экспоненциальную задержку для отправки повторного запроса при ошибке
					// 1-я попытка будет выполнена через 1 сек
					// 2-я - через 3 сек
					// 3-я - через 7 сек
						return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) - 1);
					}));

        services.AddHttpClient<CurrencyService>()
			.AddAuditHandler(audit => audit.
			IncludeRequestHeaders().
			IncludeRequestBody().
			IncludeResponseBody().
			IncludeResponseHeaders().
			IncludeContentHeaders());

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/audit.json", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _ = Configuration.Setup()
            .UseSerilog(
            config => config.Message(
            auditEvent =>
            {
                if (auditEvent is AuditEventHttpClient httpClientEvent)
                {
                    var contentBody = httpClientEvent.Action?.Response?.Content?.Body;
                    if (contentBody is string { Length: > 1000 } stringBody)
                    {
                        httpClientEvent.Action.Response.Content.Body = stringBody[..1000] + "<...>";
                    }
                }
                return auditEvent.ToJson();
            }));

        Configuration.AddCustomAction(
            ActionType.OnEventSaving,
            HideCredentials);

        void HideCredentials(AuditScope scope)
        {

            var httpAction = scope.GetHttpAction();
            if (httpAction is not null)
            {
                HideAuthorizationHeader(httpAction.Request?.Headers);
                HideAuthorizationHeader(httpAction.Response?.Headers);
            }

            void HideAuthorizationHeader(IDictionary<string, string>? headers)
            {
                if (headers?.ContainsKey("apikey") is true)
                {
                    headers["apikey"] = "*hidden*";
                }
            }
        }

		services.AddControllers(options =>
		{
			options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
		})      
		.AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                });

        services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(
			с =>
			{
				с.SwaggerDoc(
					"v1",
					new OpenApiInfo()
					{
						Title = "CurrencyAPI",
						Version = "v1",
						Description = "API для получения курса валют с внешнего сервиса"
					});
				с.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);
			});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

        app.UseRouting()
			.UseMiddleware<RequestLoggingMiddleware>()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}