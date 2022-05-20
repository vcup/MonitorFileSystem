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
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<GrpcStartup>();
    });

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
commandBuilder.AddMiddleware(content  =>
{
    if (content.ParseResult.HasOption(configPath))
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(content.ParseResult.GetValueForOption(configPath));
        });
    }
    else
    {
        builder.ConfigureAppConfiguration(UseDefaultConfigures);
    }
});

var parser = commandBuilder.Build();
await parser.InvokeAsync(args);

void UseDefaultConfigures(HostBuilderContext context,IConfigurationBuilder config)
{
    const string configName = "settings.json";
    var configNameWithEnv = "settings." + context.HostingEnvironment.EnvironmentName + ".json";
    var userConfig = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config", "monitorfs");
    config
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
}