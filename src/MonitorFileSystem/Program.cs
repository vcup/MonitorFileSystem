using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Grpc;

var builder = new HostBuilder()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables("MFS_"); // MFS -> MonitorFileSystem
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });

var rootCommand = new RootCommand();

var configPath = new Option<string>("--conf")
{
    Arity = ArgumentArity.ZeroOrOne
};

rootCommand.AddOption(configPath);
rootCommand.SetHandler(async () =>
{
    using var host = builder.Build();
    await host.RunAsync();
});


var commandBuilder = new CommandLineBuilder(rootCommand);
commandBuilder.AddMiddleware(content =>
{
    if (content.ParseResult.HasOption(configPath))
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var path = content.ParseResult.GetValueForOption(configPath);
            if (path is not null) config.AddYamlFile(path, false, true);
        });
    }
    else
    {
        builder.ConfigureAppConfiguration(UseDefaultConfigures);
    }
});

var parser = commandBuilder.Build();
await parser.InvokeAsync(args);

void UseDefaultConfigures(HostBuilderContext context, IConfigurationBuilder config)
{
#if Linux
    const string linuxConfigsPath = "/etc/monitorfs";
#endif
    const string configName = "settings.yaml";
    var configNameWithEnv = "settings." + context.HostingEnvironment.EnvironmentName + ".yaml";
    var userConfig = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config", "monitorfs");
    config
        .AddYamlFile(configName, true, true)
        .AddYamlFile(configNameWithEnv, true, true)
        .AddYamlFile(userConfig + configName, true, true)
        .AddYamlFile(userConfig + configNameWithEnv, true, true)
#if Windows
#elif Linux
        .AddYamlFile(Path.Join(linuxConfigsPath, configName), true, true)
        .AddYamlFile(Path.Join(linuxConfigsPath, configNameWithEnv), true, true)
#elif MAC
#endif
        ;
}