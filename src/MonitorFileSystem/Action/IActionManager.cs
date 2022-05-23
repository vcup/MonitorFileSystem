using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Action;

public interface IActionManager
{
    void Add(IOperate operate);

    void Add(IChain chain);

    bool RemoveOperate(Guid guid);

    bool RemoveChain(Guid guid);

    bool ContainsOperate(Guid guid);

    bool ContainsChain(Guid guid);

    bool TryGet(Guid guid, [MaybeNullWhen(false)] out IOperate operate);

    bool TryGet(Guid guid, [MaybeNullWhen(false)] out IChain operate);

    bool TryAddOperateToChain(Guid operateGuid, Guid chainGuid);

    bool TryRemoveOperateFromChain(Guid operateGuid, Guid chainGuid);

    void ClearOperates();
    void ClearChains();

    IEnumerable<IChain> Chains { get; }
    IEnumerable<IOperate> Operates { get; }
}