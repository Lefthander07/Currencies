using System.Text.Json.Serialization;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Middlewares;

using Microsoft.OpenApi.Models;
using Serilog;
using Audit.Http;
using Audit.Core;
using Grpc;
using Serilog.Configuration;
using Fuse8.BackendIntership.InternalApi.GrpcContracts;
using Fuse8.BackendInternship.InternalApi.gRPC;

namespace Fuse8.BackendInternship.InternalApi;


public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true; 
        });

        services.AddControllers(options =>
        {
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

        services.AddOptions<Ports>()
            .Bind(_configuration.GetSection("Ports"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
                с =>
                {
                    с.SwaggerDoc(
                        "v1",
                        new OpenApiInfo()
                        {
                            Title = "InternalCurrencyAPI",
                            Version = "v1",
                            Description = "API для получения курса валют с внешнего сервиса"
                        });
                    с.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);
                });

        services.AddHttpClient<CurrencyHttpApi>()
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

        services.AddTransient<CashedCurrency>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Internal API v1");
                options.RoutePrefix = "";
            });
        }

        var RESTport = _configuration.GetValue<int>("Ports:REST");

        app.UseRouting();
        app.UseWhen(context => context.Connection.LocalPort == RESTport, appBuilder =>
        {
            appBuilder.UseMiddleware<RequestLoggingMiddleware>();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<CurrencyService>();
            endpoints.MapControllers();
        });

    }
}