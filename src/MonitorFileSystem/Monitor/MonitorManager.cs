using System.Collections;

namespace MonitorFileSystem.Monitor;

public class MonitorManager : IMonitorManager
{
    private readonly List<IWatcher> _watchers = new();
    private readonly List<IGroup> _groups = new();

    private readonly Dictionary<string, IWatcher> _watcherCaches = new();
    private readonly Dictionary<string, IGroup> _groupCaches = new();

    public IWatcher? FindWatcher(string name)
    {
        if (_watcherCaches.TryGetValue(name, out var watcher))
        {
            return watcher;
        }
        watcher = _watchers.FindLast(w => w.Name == name);

        if (watcher is not null)
        {
            _watcherCaches.Add(name, watcher);
        }

        return watcher;
    }
    
    public IGroup? FindGroup(string name)
    {
        if (_groupCaches.TryGetValue(name, out var group))
        {
            return group;
        }

        group = _groups.FindLast(g => g.Name == name);

        if (group is not null)
        {
            _groupCaches.Add(name, group);
        }

        return group;
    }
    
    public void Add(IWatcher item)
    {
        if (Contains(item))
        {
            _watchers.Add(item);
        }
    }

    public void Add(IGroup item)
    {
        if (Contains(item))
        {
            _groups.Add(item);
        }
    }

    void ICollection<IWatcher>.Clear()
    {
        _watchers.Clear();
    }

    void ICollection<IGroup>.Clear()
    {
        _groups.Clear();
    }

    public void ClearUp()
    {
        (this as ICollection<IWatcher>).Clear();
        (this as ICollection<IGroup>).Clear();
    }

    public IEnumerable<IWatcher> Watchers => _watchers;
    public IEnumerable<IGroup> Groups => _groups;

    public bool Contains(IWatcher item)
    {
        return _watchers.Contains(item) || _watchers.Any(w => w.Name == item.Name);
    }

    public bool Contains(IGroup item)
    {
        return _groups.Contains(item) || _watchers.Any(g => g.Name == item.Name);
    }    
    
    public void CopyTo(IWatcher[] array, int arrayIndex)
    {
        _watchers.CopyTo(array, arrayIndex);
    }

    public void CopyTo(IGroup[] array, int arrayIndex)
    {
        _groups.CopyTo(array, arrayIndex);
    }
    
    public bool Remove(IWatcher item)
    {
        return _watchers.Remove(item) || _watchers.RemoveAll(w => w.Name == item.Name) != 0;
    }

    public bool Remove(IGroup item)
    {
        return _groups.Remove(item) || _groups.RemoveAll(g => g.Name == item.Name) != 0;
    }
    
    IEnumerator<IWatcher> IEnumerable<IWatcher>.GetEnumerator()
    {
        return _watchers.GetEnumerator();
    }
    
    IEnumerator<IGroup> IEnumerable<IGroup>.GetEnumerator()
    {
        return _groups.GetEnumerator();
    }
    
    public IEnumerator GetEnumerator()
    {
        return _watchers.GetEnumerator();
    }

    int ICollection<IWatcher>.Count => _watchers.Count;
    
    int ICollection<IGroup>.Count => _groups.Count;

    public bool IsReadOnly => false;
}