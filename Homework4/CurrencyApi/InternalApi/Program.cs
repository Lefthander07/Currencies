using Fuse8.BackendInternship.InternalApi;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;


var webHost = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>()
            .UseKestrel((context, options) =>
            {
                var grpcPort = context.Configuration.GetValue<int>("Ports:GRPC");
                var restPort = context.Configuration.GetValue<int>("Ports:REST");

                options.ListenAnyIP(restPort);
                options.ListenAnyIP(grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
    })
    .UseSerilog()
    .Build();

await webHost.RunAsync();