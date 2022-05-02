using MonitorFileSystem.Monitor;

namespace MonitorFileSystem;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var w = new MonitorManager
        {
            new Watcher("w1", "./", string.Empty),
            new Watcher("w2", "./", string.Empty),
            new Group("g1", string.Empty)
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _logger.LogInformation("Watcher Count {0}", (w as ICollection<IWatcher>).Count);
            _logger.LogInformation("Group Count {0}", (w as ICollection<IGroup>).Count);
            _logger.LogInformation("Watcher Count {0}", w.Count);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
