using MonitorFileSystem.Action;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Grpc;

public static class ActionManageExtensions
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
            Destination = operate.Destination,
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
                result.Unpack = new UnpackOperateResponse
                {
                    Guid = unpackOperate.Guid.ToString(),
                    Destination = unpackOperate.Destination,
                    Description = unpackOperate.Description,
                };
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
}