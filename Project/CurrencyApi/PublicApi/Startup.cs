﻿using Audit.Http;
using Audit.Core;
using Microsoft.OpenApi.Models;
using Serilog;
using Fuse8.BackendInternship.ModelBinders;
using Fuse8.BackendInternship.PublicApi.JsonConverters;
using Fuse8.BackendInternship.PublicApi.Middlewares;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.gRPC;
using Fuse8.BackendInternship.PublicApi.gRPC;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Fuse8.BackendInternship.PublicApi.Data;
using Fuse8.BackendInternship.PublicApi.Services;

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
            options.Filters.Add<CurrencyExceptionFilter>();
            options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        });

		services.AddOptions<CurrencyOptions>()
			.Bind(_configuration.GetSection("Currency"))
			.ValidateDataAnnotations()
			.ValidateOnStart();

        services.AddOptions<grpcUrlOptions>()
            .Bind(_configuration.GetSection("GRPC"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddGrpcClient<GrpcCurrency.GrpcCurrencyClient>((provider, options) =>
        {
            var settings = provider.GetRequiredService<IOptions<grpcUrlOptions>>();
            options.Address = new Uri(settings.Value.Url);
        }).AddAuditHandler(audit => audit.
            IncludeRequestHeaders().
            IncludeRequestBody().
            IncludeResponseBody().
            IncludeResponseHeaders().
            IncludeContentHeaders());

        services.AddScoped<CurrencyClient>();

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

        services.AddDbContext<SelectedCurrenciesDbContext>(
            optionsBuilder =>
            {
                optionsBuilder
                .UseNpgsql(
                    connectionString: _configuration.GetConnectionString("SelectedCurrency"),
                    npgsqlOptionsAction: sqlOptionBuilder =>
                    {
                        sqlOptionBuilder.EnableRetryOnFailure();
                        sqlOptionBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SelectedCurrenciesDbContext.SchemaName);
                    })
                .UseSnakeCaseNamingConvention();
                optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine);
            });
         services.AddScoped<SelectedCurrenciesRepository>();
         services.AddScoped<SelectedExchangeRatesService>();
    }

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Public API v1");
                options.RoutePrefix = "";
            });
        }

        app.UseRouting()
			.UseMiddleware<RequestLoggingMiddleware>()
			.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}