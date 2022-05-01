using MonitorFileSystem;
using MonitorFileSystem.Action;
using MonitorFileSystem.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddActions()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
