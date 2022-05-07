using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Monitor;

public interface IMonitorManager
{
    void Add(IWatcher item);
    
    void Add(IGroup item);

    bool RemoveWatcher(Guid guid);
    
    bool RemoveGroup(Guid guid);

    bool TryGet(Guid guid, [MaybeNullWhen(false)] out IWatcher watcher);
    
    bool TryGet(Guid guid, [MaybeNullWhen(false)] out IGroup group);

    bool TryAddWatcherToGroup(Guid watcherGuid, Guid groupGuid);
    
    bool TryRemoveWatcherFromGroup(Guid watcherGuid, Guid groupGuid);

    void ClearWatchers();
    void ClearGroups();
    
    IEnumerable<IWatcher> Watchers { get; }
    IEnumerable<IGroup> Groups { get; }
}