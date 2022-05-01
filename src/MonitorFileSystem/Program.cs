using MonitorFileSystem;
using MonitorFileSystem.Action;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<MoveOperate>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
