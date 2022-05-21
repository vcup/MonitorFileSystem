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

var builder = new CommandLineBuilder(rootCommand);

builder.AddMiddleware(content =>
{
    if (content.ParseResult.HasOption(GlobalOptions.ConfigPath))
    {
        var path = content.ParseResult.GetValueForOption(GlobalOptions.ConfigPath);
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
#if Windows
        .AddYamlFile(configName, true, true)
#elif Linux
        .AddYamlFile(configName, true, true)
        .AddYamlFile(Path.Join(linuxConfigsPath, configName), true, true)
#endif
        .AddYamlFile(userConfig + configName, true, true)
        .Build();
}