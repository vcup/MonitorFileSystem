using MonitorFileSystem.Client;
using MonitorFileSystem.Client.Commands;
using MonitorFileSystem.Client.Grpc;

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
            .AddScoped<GrpcSettings>()
            .AddScoped<GlobalOptions>()
            .AddGrpcClients()
            .AddHostedService<Worker>()
            ;
    })
    .Build();

await host.RunAsync();
