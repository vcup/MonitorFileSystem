namespace MonitorFileSystem.Monitor;

public static class WatcherExtension
{    
    public static WatchingEventInfo GetWatchedEventInfo(this IWatcher watcher, FileSystemEventArgs e)
    {
        WatchingEvent watchedEvent = WatchingEvent.None;

        if (e.ChangeType.HasFlag(WatcherChangeTypes.Created))
        {
            watchedEvent |= WatchingEvent.Created;
        }
        if (e.ChangeType.HasFlag(WatcherChangeTypes.Deleted))
        {
            watchedEvent |= WatchingEvent.Deleted;
        }
        if (e.ChangeType.HasFlag(WatcherChangeTypes.Changed))
        {
            watchedEvent |= watcher.WatchingEvent & (WatchingEvent)0b1111_1111_1111_0000;
        }

        var info = new WatchingEventInfo()
        {
            Watcher = watcher,
            Path = e.FullPath,
            WatchedEvent = watchedEvent,
        };

        return info;
    }

    public static WatchingEventInfo GetWatchedEventInfo(this IWatcher watcher, RenamedEventArgs e)
    {
        var info = watcher.GetWatchedEventInfo(e as FileSystemEventArgs);

        if (e.ChangeType.HasFlag(WatcherChangeTypes.Renamed))
        {
            info.WatchedEvent |= WatchingEvent.Renamed;
            info.Path = $"{e.Name ?? e.FullPath}\n{info.Path}";
        }

        return info;
    }
}
