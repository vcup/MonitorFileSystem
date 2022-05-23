using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Configuration;
using MonitorFileSystem.Client.Commands;
using MonitorFileSystem.Client.Configures;

var rootCommand = new RootCommand("commandline client of monitor file system service");

rootCommand.AddGlobalOptions()
    .AddWatchCommands()
    .AddOperateCommands()
    .AddLinkCommands()
    ;

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults();

builder.AddMiddleware(context =>
{
    if (context.ParseResult.HasOption(GlobalOptions.ConfigPath))
    {
        var path = context.ParseResult.GetValueForOption(GlobalOptions.ConfigPath);
        if (path is null) return;

        var config = new ConfigurationBuilder()
            .AddYamlFile(path)
            .Build();

        Configure.SetupSettings(config);
    }
    else
    {
        Configure.SetupSettings(DefaultConfiguration());
    }
});

builder.AddMiddleware(context =>
{
    if (context.ParseResult.HasOption(GlobalOptions.GrpcAddress))
    {
        var address = context.ParseResult.GetValueForOption(GlobalOptions.GrpcAddress);
        Configure.GrpcSettings.AddressString = address!;
    }
});

var parser = builder.Build();
return await parser.InvokeAsync(args);

IConfiguration DefaultConfiguration()
{
#if Linux
    const string linuxConfigsPath = "/etc/monitorfsc";
#endif
    const string configName = "config.yaml";
    var userConfig = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config", "monitorfsc");

    return new ConfigurationBuilder()
        .AddYamlFile(configName, true, true)
        .AddYamlFile(userConfig + configName, true, true)
#if Windows
#elif Linux
        .AddYamlFile(Path.Join(linuxConfigsPath, configName), true, true)
#elif MAC
#endif
        .Build();
}