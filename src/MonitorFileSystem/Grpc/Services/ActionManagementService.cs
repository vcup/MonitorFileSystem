﻿using System.Diagnostics;
using System.IO.Abstractions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Action;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Grpc.Services;

public class ActionManagementService : ActionManagement.ActionManagementBase
{
    private readonly ILogger<ActionManagementService> _logger;
    private readonly IActionManager _manager;
    private readonly IServiceProvider _provider;

    public ActionManagementService(ILogger<ActionManagementService> logger, IActionManager manager, IServiceProvider provider)
    {
        _logger = logger;
        _manager = manager;
        _provider = provider;
    }

    public override Task<MoveOperateResponse> CreateMoveOperate(MoveOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetService<IMoveOperate>();
            Debug.Assert(operate is not null);
            operate.Initialization(request.Destination);
            
            return new MoveOperateResponse
            {
                Guid = operate.Guid.ToString(),
                Destination = operate.Destination,
                Description = operate.Description = request.Description
            };
        });
    }

    public override Task<UnpackOperateResponse> CreateUnpackOperate(UnpackOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetService<IUnpackOperate>();
            Debug.Assert(operate is not null);
            operate.Initialization();
            
            if (!string.IsNullOrEmpty(request.Destination))
            {
                operate.Destination = request.Destination;
            }

            return new UnpackOperateResponse
            {
                Guid = operate.Guid.ToString(),
                Destination = operate.Destination ?? string.Empty,
                Description = operate.Description = request.Description
            };
        });
    }

    public override Task<Empty> RemoveOperate(OperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.Remove(Guid.Parse(request.Guid));
            return new Empty();
        });
    }

    public override Task<Empty> UpdateOperate(OperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryGetValue(Guid.Parse(request.Guid), out var operate);
            Debug.Assert(operate is not null);
            operate.Description = request.Description;
            
            return new Empty();
        });
    }

    public override Task<Empty> UpdateMoveOperate(UpdateMoveOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryGetValue(Guid.Parse(request.Guid), out var value);
            Debug.Assert(value is not null);
            Debug.Assert(value is IMoveOperate);
            
            var operate = (IMoveOperate)value;
            operate.Description = request.Description;
            operate.Destination = request.Destination;
            
            return new Empty();
        });
    }

    public override Task<Empty> UpdateUnpackOperate(UpdateMoveOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryGetValue(Guid.Parse(request.Guid), out var value);
            Debug.Assert(value is not null);
            Debug.Assert(value is IMoveOperate);
            
            var operate = (IUnpackOperate)value;
            operate.Destination = string.IsNullOrEmpty(request.Destination) ? null : request.Destination;
            operate.Description = request.Description;
            
            return new Empty();
        });
    }

    public override Task<ChainResponse> CreateChain(ChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetService<IChain>();
            Debug.Assert(operate is not null);
            operate.Initialization(request.Name);

            return new ChainResponse
            {
                Guid = operate.Guid.ToString(),
                Name = operate.Name,
                Description = operate.Description = request.Description
            };
        });
    }

    public override Task<Empty> RemoveChain(ChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.Remove(request.Name);
            return new Empty();
        });
    }

    public override Task<Empty> AddOperateTo(OperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var chain = _manager.Find(request.Chain.Name);
            _manager.TryGetValue(Guid.Parse(request.Operate.Guid), out var operate);
            Debug.Assert(chain is not null);
            Debug.Assert(operate is not null);
            chain.Add(operate);
            
            return new Empty();
        });
    }

    public override Task<Empty> AddOperateToMany(OperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryGetValue(Guid.Parse(request.Operate.Guid), out var operate);
            Debug.Assert(operate is not null);
            
            foreach (var chainRequest in request.Chains)
            {
                var chain = _manager.Find(chainRequest.Name);
                Debug.Assert(chain is not null);
                chain.Add(operate);
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> AddManyOperateTo(ManyOperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var chain = _manager.Find(request.Chain.Name);
            Debug.Assert(chain is not null);

            foreach (var operateRequest in request.Operates)
            {
                _manager.TryGetValue(Guid.Parse(operateRequest.Guid), out var operate);
                Debug.Assert(operate is not null);
                chain.Add(operate);
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> AddManyOperateToMany(ManyOperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var chainRequest in request.Chains)
            {
                var chain = _manager.Find(chainRequest.Name);
                Debug.Assert(chain is not null);

                foreach (var operateRequest in request.Operates)
                {
                    _manager.TryGetValue(Guid.Parse(operateRequest.Guid), out var operate);
                    Debug.Assert(operate is not null);
                    chain.Add(operate);
                }
            }
            return new Empty();
        });
    }

    public override Task<Empty> RemoveOperateFor(OperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var chain = _manager.Find(request.Chain.Name);
            _manager.TryGetValue(Guid.Parse(request.Operate.Guid), out var operate);
            Debug.Assert(chain is not null);
            Debug.Assert(operate is not null);
            chain.Remove(operate);
            
            return new Empty();
        });
    }

    public override Task<Empty> RemoveOperateForMany(OperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryGetValue(Guid.Parse(request.Operate.Guid), out var operate);
            Debug.Assert(operate is not null);
            
            foreach (var chainRequest in request.Chains)
            {
                var chain = _manager.Find(chainRequest.Name);
                Debug.Assert(chain is not null);
                chain.Remove(operate);
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> RemoveManyOperateFor(ManyOperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var chain = _manager.Find(request.Chain.Name);
            Debug.Assert(chain is not null);

            foreach (var operateRequest in request.Operates)
            {
                _manager.TryGetValue(Guid.Parse(operateRequest.Guid), out var operate);
                Debug.Assert(operate is not null);
                chain.Remove(operate);
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> RemoveManyOperateForMany(ManyOperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var chainRequest in request.Chains)
            {
                var chain = _manager.Find(chainRequest.Name);
                Debug.Assert(chain is not null);

                foreach (var operateRequest in request.Operates)
                {
                    _manager.TryGetValue(Guid.Parse(operateRequest.Guid), out var operate);
                    Debug.Assert(operate is not null);
                    chain.Remove(operate);
                }
            }
            return new Empty();
        });
    }

}