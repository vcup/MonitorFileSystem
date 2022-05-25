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

    public static string GetId(this IWatcher watcher)
    {
        return string.IsNullOrEmpty(watcher.Name) ? watcher.Guid.ToString() : watcher.Name;
    }

    public static string GetDetail(this IWatcher watcher)
    {
        return string.Format("Guid:                {0}\n" +
                             "monitoring path:     {1}\n" +
                             "Filter:              {2}\n" +
                             "Event:               {3}\n" +
                             "MonitorSubDirectory: {4}\n",
            watcher.Guid.ToString(), watcher.MonitorPath, watcher.Filter, watcher.WatchingEvent,
            watcher.MonitorSubDirectory);
    }


    public static void Initialization(this IWatcher watcher, string path)
    {
        watcher.Initialization();
        watcher.MonitorPath = path;
    }

    public static void Initialization(this IWatcher watcher, string path, string filter)
    {
        watcher.Initialization();
        watcher.MonitorPath = path;
        watcher.Filter = filter;
    }

    public static void Initialization(this IWatcher watcher, string name, string path, string filter)
    {
        watcher.Initialization();
        watcher.Name = name;
        watcher.MonitorPath = path;
        watcher.Filter = filter;
    }

    public static void Initialization(this IWatcher watcher, WatchingEvent @event)
    {
        watcher.Initialization();
        watcher.WatchingEvent = @event;
    }

    public static void Initialization(this IWatcher watcher, string path, WatchingEvent @event)
    {
        watcher.Initialization();
        watcher.MonitorPath = path;
        watcher.WatchingEvent = @event;
    }

    public static void Initialization(this IWatcher watcher, string path, string filter, WatchingEvent @event)
    {
        watcher.Initialization();
        watcher.MonitorPath = path;
        watcher.Filter = filter;
        watcher.WatchingEvent = @event;
    }

    public static void Initialization(this IWatcher watcher, string name, string path, string filter, WatchingEvent @event)
    {
        watcher.Initialization();
        watcher.Name = name;
        watcher.MonitorPath = path;
        watcher.Filter = filter;
        watcher.WatchingEvent = @event;
    }
}