using System.Text.Json.Serialization;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Middlewares;
using Microsoft.OpenApi.Models;
using Serilog;
using Audit.Http;
using Audit.Core;
using Fuse8.BackendInternship.InternalApi.gRPC;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.ModelBinders;
using Fuse8.BackendInternship.InternalApi.JsonConverters;
using Fuse8.BackendInternship.InternalApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

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
            options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
        })
        // Добавляем глобальные настройки для преобразования Json
        .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());

                    });

        services.AddOptions<CurrencyHttpApiOptions>()
            .Bind(_configuration.GetSection("CurrencyHttpApi"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CurrencyOptions>()
            .Bind(_configuration.GetSection("Currency"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<PortsOptions>()
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

        Configuration.Setup()
        .UseSerilog(
            config => config
            .LogLevel(
                auditEvent =>
            {
                if (auditEvent is AuditEventHttpClient)
                {
                    return Audit.Serilog.LogLevel.Debug;
                }

                return Audit.Serilog.LogLevel.Info;
            }).Message(
                AuditEvent =>
                {
                    AuditEvent.Environment = null;

                    const int MaxAuditContentLength = 10_000;
                    if (AuditEvent is AuditEventHttpClient httpClientEvent)
                    {
                        var responseContent = httpClientEvent.Action?.Response?.Content;
                        if (responseContent is { Body: string { Length: > MaxAuditContentLength } bodyContent })
                        {
                            responseContent.Body = bodyContent[..MaxAuditContentLength] + "<...>";
                        }
                    }
                    return AuditEvent.ToJson();
                }));
        Configuration.AddCustomAction(
            ActionType.OnEventSaving,
            HideCredentials);
        Configuration.JsonSettings = new System.Text.Json.JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

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
        services.AddDbContext<CurrencyDbContext>(
            optionsBuilder =>
            {
                optionsBuilder
                .UseNpgsql(
                    connectionString: _configuration.GetConnectionString("CurrencyCache"),
                    npgsqlOptionsAction: sqlOptionBuilder =>
                    {
                        sqlOptionBuilder.EnableRetryOnFailure();
                        sqlOptionBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "cur");
                    })
                .UseSnakeCaseNamingConvention();
                optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine);
            }
        );
            
       // services.AddScoped<ICachedCurrencyAPI, CashedCurrency>();
        services.AddScoped<ICachedCurrencyAPI, CashedCurrency_DB>();
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
        var GRPCport = _configuration.GetValue<int>("Ports:GRPC");

            app.UseWhen(
            predicate: context => context.Connection.LocalPort == GRPCport,
            configuration: grpcBuilder =>
            {
                grpcBuilder.UseRouting();
                grpcBuilder.UseEndpoints(endpoints => endpoints.MapGrpcService<CurrencyService>());
            });

            app.UseWhen(
            predicate: context => context.Connection.LocalPort == RESTport,
            configuration: restBuilder =>
            {
                restBuilder.UseRouting();
                restBuilder.UseMiddleware<RequestLoggingMiddleware>();
                restBuilder.UseEndpoints(endpoints => endpoints.MapControllers());
            });
    }
}