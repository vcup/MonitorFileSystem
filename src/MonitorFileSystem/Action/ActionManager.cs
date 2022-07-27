using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Action;

public class ActionManager : IActionManager
{
    private readonly Dictionary<Guid, IOperate> _operates = new();
    private readonly Dictionary<Guid, IChain> _chains = new();

    public void Add(IOperate operate)
    {
        _operates.Add(operate.Guid, operate);
    }

    public void Add(IChain chain)
    {
        _chains.Add(chain.Guid, chain);
    }

    public bool RemoveOperate(Guid guid)
    {
        return _operates.Remove(guid);
    }

    public bool RemoveChain(Guid guid)
    {
        return _chains.Remove(guid);
    }

    public bool ContainsOperate(Guid guid)
    {
        return _operates.ContainsKey(guid);
    }

    public bool ContainsChain(Guid guid)
    {
        return _chains.ContainsKey(guid);
    }

    public bool TryGet(Guid guid, [MaybeNullWhen(false)] out IOperate operate)
    {
        return _operates.TryGetValue(guid, out operate);
    }

    public bool TryGet(Guid guid, [MaybeNullWhen(false)] out IChain operate)
    {
        return _chains.TryGetValue(guid, out operate);
    }

    public bool TryAddOperateToChain(Guid operateGuid, Guid chainGuid)
    {
        if (!this.TryGetOperate(operateGuid, out var operate) ||
            !this.TryGetChain(chainGuid, out var chain))
        {
            return false;
        }

        chain.Add(operate);

        return true;
    }

    public bool TryRemoveOperateFromChain(Guid operateGuid, Guid chainGuid)
    {
        if (!this.TryGetOperate(operateGuid, out var operate) ||
            !this.TryGetChain(chainGuid, out var chain))
        {
            return false;
        }

        chain.Remove(operate);

        return true;
    }

    public void ClearOperates()
    {
        _operates.Clear();
    }

    public void ClearChains()
    {
        _chains.Clear();
    }

    public IEnumerable<IChain> Chains => _chains.Values;
    public IEnumerable<IOperate> Operates => _operates.Values;
}