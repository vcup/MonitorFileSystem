using MonitorFileSystem.Client.Commands;

namespace MonitorFileSystem.Client;

public class Worker : BackgroundService
{
    private readonly CommandTree _commandTree;
    private readonly IHostApplicationLifetime _lifetime;
    
    public Worker(CommandTree commandTree, IHostApplicationLifetime lifetime)
    {
        _commandTree = commandTree;
        _lifetime = lifetime;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _commandTree.InvokeAsync();
        _lifetime.StopApplication();
    }
}