using Fuse8.BackendInternship.PublicApi;
using Microsoft.AspNetCore;

var webHost = WebHost
	.CreateDefaultBuilder(args)
	.UseStartup<Startup>()
	.Build();

await webHost.RunAsync();