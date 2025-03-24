using Audit.Http;
using Audit.Core;
using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Fuse8.BackendInternship.PublicApi;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
		Console.WriteLine(configuration["API_KEY"]);
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

		services.AddControllers(options =>
		{
			options.Filters.Add<CurrencyExceptionFilter>();
		});

		var section = _configuration.GetRequiredSection("currency");
		services.Configure<CurrencySettings>(section);

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

        Configuration.Setup()
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
				//Log.Information(auditEvent.ToJson());
				return auditEvent.ToJson();
			}));

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
        app.UseMiddleware<RequestLogging>();
        app.UseRouting()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}