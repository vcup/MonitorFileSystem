using System.Diagnostics.CodeAnalysis;
using MonitorFileSystem.Action;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Grpc;

public static class ActionManagementExtensions
{
    public static MoveOperateResponse ToResponse(this IMoveOperate operate)
    {
        return new MoveOperateResponse
        { 
            Guid = operate.Guid.ToString(),
            Destination = operate.Destination,
            Description = operate.Description
        };
    }
    
    public static UnpackOperateResponse ToResponse(this IUnpackOperate operate)
    {
        return new UnpackOperateResponse
        { 
            Guid = operate.Guid.ToString(),
            Destination = operate.Destination ?? String.Empty,
            Description = operate.Description
        };
    }
    
    public static OperateResponse ToResponse(this IOperate operate)
    {
        var result = new OperateResponse();
        switch (operate)
        {
            case IMoveOperate moveOperate:
                result.Move = moveOperate.ToResponse();
                break;
            case IUnpackOperate unpackOperate:
                result.Unpack = unpackOperate.ToResponse();
                break;
        }

        return result;
    }
    
    public static ChainResponse ToResponse(this IChain chain)
    {
        return new ChainResponse
        {
            Guid = chain.Guid.ToString(),
            Name = chain.Name,
            Description = chain.Description
        };
    }

    public static bool TryGetOperate(this IActionManager manager, OperateRequest request,
        [MaybeNullWhen(false)] out IOperate operate)
    {
        return manager.TryGet(Guid.Parse(request.Guid), out operate);
    }
    
    public static bool TryGetOperate(this IActionManager manager, GuidRequest request,
        [MaybeNullWhen(false)] out IOperate operate)
    {
        return manager.TryGet(Guid.Parse(request.Guid), out operate);
    }

    public static bool TryGetChain(this IActionManager manager, GuidRequest request,
        [MaybeNullWhen(false)] out IChain chain)
    {
        return manager.TryGet(Guid.Parse(request.Guid), out chain);
    }

    public static bool TryAddOperateToChain(this IActionManager manager, OperateAndChainRequest request)
    {
        return manager.TryAddOperateToChain(Guid.Parse(request.Operate.Guid), Guid.Parse(request.Chain.Guid));
    }

    public static bool TryAddOperateToChain(this IActionManager manager, OperateAndManyChainRequest request)
    {
        var operateGuid = Guid.Parse(request.Operate.Guid);

        return request.Chains.All(chain => manager.TryAddOperateToChain(operateGuid, Guid.Parse(chain.Guid)));
    }

    public static bool TryAddOperateToChain(this IActionManager manager, ManyOperateAndChainRequest request)
    {
        var chainGuid = Guid.Parse(request.Chain.Guid);
        
        return request.Operates.All(operate => manager.TryAddOperateToChain(Guid.Parse(operate.Guid), chainGuid));
    }

    public static bool TryAddOperateToChain(this IActionManager manager, ManyOperateAndManyChainRequest request)
    {
        var operateGuids = request.Operates.Select(operate => Guid.Parse(operate.Guid));
        var chainGuids = request.Chains.Select(chain => Guid.Parse(chain.Guid));

        return operateGuids.Zip(chainGuids)
            .All(union => manager.TryAddOperateToChain(union.First, union.Second));
    }
    
    public static bool TryRemoveOperateFromChain(this IActionManager manager, OperateAndChainRequest request)
    {
        return manager.TryRemoveOperateFromChain(Guid.Parse(request.Operate.Guid), Guid.Parse(request.Chain.Guid));
    }

    public static bool TryRemoveOperateFromChain(this IActionManager manager, OperateAndManyChainRequest request)
    {
        var operateGuid = Guid.Parse(request.Operate.Guid);

        return request.Chains.All(chain => manager.TryRemoveOperateFromChain(operateGuid, Guid.Parse(chain.Guid)));
    }

    public static bool TryRemoveOperateFromChain(this IActionManager manager, ManyOperateAndChainRequest request)
    {
        var chainGuid = Guid.Parse(request.Chain.Guid);
        
        return request.Operates.All(operate => manager.TryRemoveOperateFromChain(Guid.Parse(operate.Guid), chainGuid));
    }

    public static bool TryRemoveOperateFromChain(this IActionManager manager, ManyOperateAndManyChainRequest request)
    {
        var operateGuids = request.Operates.Select(operate => Guid.Parse(operate.Guid));
        var chainGuids = request.Chains.Select(chain => Guid.Parse(chain.Guid));

        return operateGuids.Zip(chainGuids)
            .All(union => manager.TryRemoveOperateFromChain(union.First, union.Second));
    }
}