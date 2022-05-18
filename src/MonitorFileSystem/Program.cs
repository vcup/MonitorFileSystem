using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Grpc;

var w = Host.CreateDefaultBuilder();

var builder = new HostBuilder()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables("MFS_");
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        const string configName = "settings.json";
        var configNameWithEnv = "settings." + context.HostingEnvironment.EnvironmentName + ".json";
        var userConfig = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config", "monitorfs");

        config.AddEnvironmentVariables("MFS_") // MFS -> MonitorFileSystem
#if Windows
            .AddJsonFile(configName, true, true)
            .AddJsonFile(configNameWithEnv, true, true)
#elif Linux
            .AddJsonFile(configName, true, true)
            .AddJsonFile(configNameWithEnv, true, true)
#endif
            .AddJsonFile(userConfig + configName, true, true)
            .AddJsonFile(userConfig + configNameWithEnv, true, true)
            ;
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    });

using var host = builder.Build();

await host.RunAsync();
