using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Grpc;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddActions()
            .AddHostedService<Worker>();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    })
    .Build();

await host.RunAsync();
