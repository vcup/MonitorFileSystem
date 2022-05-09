using System.CommandLine;
using MonitorFileSystem.Client.Commands;

namespace MonitorFileSystem.Client;

public class Worker : BackgroundService
{
    private readonly GlobalOptions _options;
    private readonly CommandLineArguments _arguments;
    private readonly IHostApplicationLifetime _lifetime;
    
    public Worker(GlobalOptions options, CommandLineArguments arguments, IHostApplicationLifetime lifetime)
    {
        _options = options;
        _arguments = arguments;
        _lifetime = lifetime;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var rootCommand = new RootCommand();
        foreach (var property in typeof(GlobalOptions).GetProperties())
        {
            var value = property.GetValue(_options);
            if (value is Option option)
            {
                rootCommand.Add(option);
            }
        }

        await rootCommand.InvokeAsync(_arguments.Arguments);
        _lifetime.StopApplication();
    }
}