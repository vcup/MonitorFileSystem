namespace MonitorFileSystem.Monitor;

public struct WatchingEventInfo
{
    public IWatcher Watcher;
    public string Path; // if Event is Renamed, Path will Start with @, use \n for split
    public WatchingEvent WatchedEvent;
}
