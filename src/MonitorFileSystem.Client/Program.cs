using MonitorFileSystem.Client;
using MonitorFileSystem.Client.Commands;

using IHost host = new HostBuilder()
    .ConfigureHostConfiguration(builder =>
    {
        builder.AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            ;
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(new CommandLineArguments(args))
            .AddSingleton<GrpcSettings>()
            .AddSingleton<GlobalOptions>()
            .AddHostedService<Worker>()
            ;
    })
    .Build();

await host.RunAsync();
