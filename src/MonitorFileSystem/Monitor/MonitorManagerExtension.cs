using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Monitor;

public static class MonitorManagerExtension
{
    public static bool TryGetWatcher(this IMonitorManager manager, Guid guid,
        [MaybeNullWhen(false)] out IWatcher watcher)
    {
        return manager.TryGet(guid, out watcher);
    }
    
    public static bool TryGetGroup(this IMonitorManager manager, Guid guid,
        [MaybeNullWhen(false)] out IGroup group)
    {
        return manager.TryGet(guid, out group);
    }

    public static bool TryGetObservable(this IMonitorManager manager, Guid guid,
        [MaybeNullWhen(false)] out IObservable<WatchingEventInfo> result)
    {
        if (manager.TryGetWatcher(guid, out var watcher))
        {
            result = watcher;
            return true;
        }

        if (manager.TryGetGroup(guid, out var group))
        {
            result = group;
            return true;
        }

        result = null;
        return false;
    }

    public static bool TryAddWatcherToGroup(this IMonitorManager manager, IWatcher watcher, Guid guid)
    {
        if (!manager.TryGetGroup(guid, out var group)) return false;
        group.Add(watcher);
        
        return true;
    }
    
    public static bool TryAddWatcherToGroup(this IMonitorManager manager, Guid guid, IGroup group)
    {
        if (!manager.TryGetWatcher(guid, out var watcher)) return false;
        group.Add(watcher);
        
        return true;
    }

    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, IWatcher watcher, Guid guid)
    {
        if (!manager.TryGetGroup(guid, out var group)) return false;
        group.Add(watcher);
        
        return true;
    }
    
    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, Guid guid, IGroup chain)
    {
        if (!manager.TryGetWatcher(guid, out var watcher)) return false;
        chain.Add(watcher);
        
        return true;
    }
}