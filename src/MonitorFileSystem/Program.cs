using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Grpc;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    })
    .Build();

await host.RunAsync();
