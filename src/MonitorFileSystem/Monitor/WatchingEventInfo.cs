namespace MonitorFileSystem.Monitor;

public sealed class WatchingEventInfo
{
    public IWatcher Watcher = null!;
    public string Path = null!; // if Event is Renamed, Path will Start with @, use \n for split
    public WatchingEvent WatchedEvent;
}