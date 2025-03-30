using Fuse8.BackendInternship.PublicApi;
using Microsoft.AspNetCore;
using Serilog;

var webHost = Host
	.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
	.UseSerilog()
	.Build();

await webHost.RunAsync();