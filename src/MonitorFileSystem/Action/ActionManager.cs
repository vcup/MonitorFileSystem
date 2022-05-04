using System.Collections;

namespace MonitorFileSystem.Action;

public class ActionManager : IActionManager
{
    private readonly List<IChain> _chains = new();
    private readonly Dictionary<string, IChain> _chainCaches = new();
    private readonly Dictionary<Guid, IOperate> _operates = new();    
    
    public Guid Add(IOperate operate)
    {
        if (Values.Contains(operate))
        {
            throw new InvalidOperationException("Operate existed!");
        }
        Guid guid = Guid.NewGuid();
        Add(guid, operate);

        return guid;
    }

    public bool Contains(string name)
    {
        return _chains.Any(c => name.Equals(c.Name));
    }

    public IChain? Find(string name)
    {
        if (_chainCaches.TryGetValue(name, out var value))
        {
            return value;
        }

        value = _chains.FindLast(c => name.Equals(c.Name));

        if (value is not null)
        {
            _chainCaches.Add(name, value);
        }

        return value;
    }

    public IEnumerable<IChain> Chains => _chains;
    public IEnumerable<IOperate> Operates => Values;
    
    public void ClearUp()
    {
        (this as IDictionary<Guid, IOperate>).Clear();
        (this as ICollection<IChain>).Clear();
    }

    public void Add(KeyValuePair<Guid, IOperate> item)
    {
        (_operates as ICollection<KeyValuePair<Guid, IOperate>>).Add(item);
    }

    public void Add(IChain item)
    {
        if (Contains(item))
        {
            _chains.Add(item);
        }
    }

    void ICollection<IChain>.Clear()
    {
        _chains.Clear();
    }

    public bool Contains(IChain item)
    {
        return _chains.Contains(item) || _chains.Any(c => item.Name.Equals(c.Name));
    }

    public void CopyTo(IChain[] array, int arrayIndex)
    {
        _chains.CopyTo(array, arrayIndex);
    }

    public bool Remove(IChain item)
    {
        return _chains.Remove(item);
    }
    
    IEnumerator<IChain> IEnumerable<IChain>.GetEnumerator()
    {
        return _chains.GetEnumerator();
    }
    
    int ICollection<IChain>.Count => _chains.Count;

    bool ICollection<IChain>.IsReadOnly => false;

    void ICollection<KeyValuePair<Guid, IOperate>>.Clear()
    {
        _operates.Clear();
    }

    public bool Contains(KeyValuePair<Guid, IOperate> item)
    {
        return (_operates as ICollection<KeyValuePair<Guid, IOperate>>).Contains(item);
    }

    public void CopyTo(KeyValuePair<Guid, IOperate>[] array, int arrayIndex)
    {
        (_operates as ICollection<KeyValuePair<Guid, IOperate>>).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<Guid, IOperate> item)
    {
        return (_operates as ICollection<KeyValuePair<Guid, IOperate>>).Remove(item);
    }

    int ICollection<KeyValuePair<Guid, IOperate>>.Count => _operates.Count;

    bool ICollection<KeyValuePair<Guid, IOperate>>.IsReadOnly => false;

    public void Add(Guid key, IOperate value)
    {
        _operates.Add(key, value);
    }

    public bool ContainsKey(Guid key)
    {
        return _operates.ContainsKey(key);
    }

    public bool Remove(Guid key)
    {
        return _operates.Remove(key);
    }

    public bool TryGetValue(Guid key, out IOperate value)
    {
        return _operates.TryGetValue(key, out value!);
    }

    public IOperate this[Guid key]
    {
        get => _operates[key];
        set => _operates[key] = value;
    }

    public ICollection<Guid> Keys => _operates.Keys;
    public ICollection<IOperate> Values => _operates.Values;
    
    IEnumerator<KeyValuePair<Guid, IOperate>> IEnumerable<KeyValuePair<Guid, IOperate>>.GetEnumerator()
    {
        return _operates.GetEnumerator();
    }
    
    public IEnumerator GetEnumerator()
    {
        return (this as IDictionary<Guid, IOperate>).GetEnumerator();
    }
}
