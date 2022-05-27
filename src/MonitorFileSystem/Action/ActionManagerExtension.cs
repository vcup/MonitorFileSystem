using System.Diagnostics.CodeAnalysis;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public static class ActionManagerExtension
{
    public static bool TryGetOperate(this IActionManager manager, Guid guid,
        [MaybeNullWhen(false)] out IOperate operate)
    {
        return manager.TryGet(guid, out operate);
    }

    public static bool TryGetOperate(this IActionManager manager, string guid,
        [MaybeNullWhen(false)] out IOperate operate)
    {
        return manager.TryGet(Guid.Parse(guid), out operate);
    }

    public static bool TryGetChain(this IActionManager manager, Guid guid,
        [MaybeNullWhen(false)] out IChain chain)
    {
        return manager.TryGet(guid, out chain);
    }

    public static bool TryGetChain(this IActionManager manager, string guid,
        [MaybeNullWhen(false)] out IChain chain)
    {
        return manager.TryGet(Guid.Parse(guid), out chain);
    }

    public static bool RemoveOperate(this IActionManager manager, string guid)
    {
        return manager.RemoveOperate(Guid.Parse(guid));
    }

    public static bool RemoveChain(this IActionManager manager, string guid)
    {
        return manager.RemoveChain(Guid.Parse(guid));
    }

    public static bool ContainsOperate(this IActionManager manager, string guid)
    {
        return manager.ContainsOperate(Guid.Parse(guid));
    }

    public static bool ContainsChain(this IActionManager manager, string guid)
    {
        return manager.ContainsChain(Guid.Parse(guid));
    }

    public static bool TryGetObserver(this IActionManager manager, Guid guid,
        [MaybeNullWhen(false)] out IObserver<WatchingEventInfo> result)
    {
        if (manager.TryGetOperate(guid, out var watcher))
        {
            result = watcher;
            return true;
        }

        if (manager.TryGetChain(guid, out var group))
        {
            result = group;
            return true;
        }

        result = null;
        return false;
    }

    public static bool TryAddOperateToChain(this IActionManager manager, IOperate operate, Guid guid)
    {
        if (!manager.TryGetChain(guid, out var chain)) return false;
        chain.Add(operate);

        return true;
    }

    public static bool TryAddOperateToChain(this IActionManager manager, Guid guid, IChain chain)
    {
        if (!manager.TryGetOperate(guid, out var operate)) return false;
        chain.Add(operate);

        return true;
    }

    public static bool TryRemoveOperateFromChain(this IActionManager manager, IOperate operate, Guid guid)
    {
        if (!manager.TryGetChain(guid, out var chain)) return false;
        chain.Add(operate);

        return true;
    }

    public static bool TryRemoveOperateFromChain(this IActionManager manager, Guid guid, IChain chain)
    {
        if (!manager.TryGetOperate(guid, out var operate)) return false;
        chain.Add(operate);

        return true;
    }
}