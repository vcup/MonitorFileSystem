using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Monitor;

public class MonitorManager : IMonitorManager
{
    private readonly Dictionary<Guid, IWatcher> _watchers = new();
    private readonly Dictionary<Guid, IGroup> _groups = new();

    public void Add(IWatcher item)
    {
        _watchers.TryAdd(item.Guid, item);
    }

    public void Add(IGroup item)
    {
        _groups.TryAdd(item.Guid, item);
    }

    public bool RemoveWatcher(Guid guid)
    {
        return _watchers.Remove(guid);
    }

    public bool RemoveGroup(Guid guid)
    {
        return _groups.Remove(guid);
    }

    public bool TryGet(Guid guid, [MaybeNullWhen(false)] out IWatcher watcher)
    {
        return _watchers.TryGetValue(guid, out watcher);
    }

    public bool TryGet(Guid guid, [MaybeNullWhen(false)] out IGroup group)
    {
        return _groups.TryGetValue(guid, out group);
    }

    public bool TryAddWatcherToGroup(Guid watcherGuid, Guid groupGuid)
    {
        if (!this.TryGetWatcher(watcherGuid, out var watcher) || !this.TryGetGroup(groupGuid, out var group))
        {
            return false;
        }

        group.Add(watcher);

        return true;
    }

    public bool TryRemoveWatcherFromGroup(Guid watcherGuid, Guid groupGuid)
    {
        if (!this.TryGetWatcher(watcherGuid, out var watcher) || !this.TryGetGroup(groupGuid, out var group))
        {
            return false;
        }

        group.Remove(watcher);
        return true;
    }


    public void ClearWatchers()
    {
        _watchers.Clear();
    }

    public void ClearGroups()
    {
        _groups.Clear();
    }

    public IEnumerable<IWatcher> Watchers => _watchers.Values;
    public IEnumerable<IGroup> Groups => _groups.Values;
}