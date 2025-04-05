using Fuse8.BackendInternship.PublicApi;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;

var webHost = Host
	.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
    .UseSerilog((context, _, LoggerConfiguration) => LoggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(
                new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithFilter(
                    new IgnorePropertyByNameExceptionFilter(
                        nameof(Exception.StackTrace),
                        nameof(Exception.Message),
                        nameof(Exception.TargetSite),
                        nameof(Exception.Source),
                        nameof(Exception.HResult), "Type"))))
        .Build();

await webHost.RunAsync();