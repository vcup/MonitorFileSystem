﻿using System.Diagnostics.CodeAnalysis;

namespace MonitorFileSystem.Action;

public static class ActionManagerExtension
{
    public static bool TryGetOperate(this IActionManager manager, Guid guid,
        [MaybeNullWhen(false)] out IOperate operate)
    {
        return manager.TryGet(guid, out operate);
    }
    
    public static bool TryGetChain(this IActionManager manager, Guid guid,
        [MaybeNullWhen(false)] out IChain chain)
    {
        return manager.TryGet(guid, out chain);
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