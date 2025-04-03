using Fuse8.BackendInternship.InternalApi;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;

var webHost = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
        .UseStartup<Startup>()
        .UseKestrel((builderContext, options) =>
        {
                var ports = builderContext.Configuration.GetSection("Ports").Get<PortsOptions>();

                options.ConfigureEndpointDefaults(p =>
                {
                    p.Protocols = p.IPEndPoint!.Port == ports.gRPC
                                ? HttpProtocols.Http2
                                : HttpProtocols.Http1;
                });
            });
        })
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