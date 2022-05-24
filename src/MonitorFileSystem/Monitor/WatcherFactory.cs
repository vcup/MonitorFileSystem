namespace MonitorFileSystem.Monitor;

public class WatcherFactory
{
    private readonly IServiceProvider _provider;

    public WatcherFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IWatcher Create()
    {
        return _provider.GetRequiredService<IWatcher>();
    }
}