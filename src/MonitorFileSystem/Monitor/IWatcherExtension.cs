namespace MonitorFileSystem.Monitor;

public static class IWatcherExtension
{    
    public static WatchingEventInfo GetWatchedEventInfo(this IWatcher watcher, FileSystemEventArgs e)
    {
        WatchingEvent WatchedEvent = WatchingEvent.None;

        if (e.ChangeType.HasFlag(WatcherChangeTypes.Created))
        {
            WatchedEvent |= WatchingEvent.Created;
        }
        if (e.ChangeType.HasFlag(WatcherChangeTypes.Deleted))
        {
            WatchedEvent |= WatchingEvent.Deleted;
        }
       if (e.ChangeType.HasFlag(WatcherChangeTypes.Changed))
        {
            WatchedEvent |= watcher.WatchingEvent & (WatchingEvent)0b1111_1111_1111_0000;
        }

        var info = new WatchingEventInfo()
        {
            watcher = watcher,
            Path = e.Name ?? e.FullPath,
            WatchedEvent = WatchedEvent,
        };

        return info;
    }

    public static WatchingEventInfo GetWatchedEventInfo(this IWatcher watcher, RenamedEventArgs e)
    {
        var info = watcher.GetWatchedEventInfo(e);

        if (e.ChangeType.HasFlag(WatcherChangeTypes.Renamed))
        {
            info.WatchedEvent |= WatchingEvent.Renamed;
            info.Path = $"{e.Name ?? e.FullPath}\n{info.Path}";
        }

        return info;
    }
}
