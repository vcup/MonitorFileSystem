using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Grpc;
using MonitorFileSystem.Monitor;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddActions()
            .AddSingleton<IMonitorManager, MonitorManager>()
            .AddSingleton<IActionManager, ActionManager>()
            ;
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    })
    .Build();

await host.RunAsync();
