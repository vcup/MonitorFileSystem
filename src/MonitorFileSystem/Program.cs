using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Grpc;
using MonitorFileSystem.Monitor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddActions()
            .AddSingleton<IMonitorManager, MonitorManager>()
            ;
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    })
    .Build();

await host.RunAsync();
