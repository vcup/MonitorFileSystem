using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Action;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using CommandOperateArgument = MonitorFileSystem.Action.CommandOperateArgument;

namespace MonitorFileSystem.Grpc.Services;

public class ActionManagementService : ActionManagement.ActionManagementBase
{
    private readonly ILogger<ActionManagementService> _logger;
    private readonly IActionManager _manager;
    private readonly IServiceProvider _provider;

    public ActionManagementService(ILogger<ActionManagementService> logger, IActionManager manager,
        IServiceProvider provider)
    {
        _logger = logger;
        _manager = manager;
        _provider = provider;
    }

    public override Task<MoveOperateResponse> CreateMoveOperate(MoveOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetRequiredService<IMoveOperate>();
            operate.Initialization(request.Destination);

            if (request.HasDescription)
            {
                operate.Description = request.Description;
            }
            _manager.Add(operate);

            return operate.ToResponse();
        });
    }

    public override Task<UnpackOperateResponse> CreateUnpackOperate(UnpackOperateRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetRequiredService<IUnpackOperate>();
            operate.Initialization();
            _manager.Add(operate);

            if (request.HasDestination)
            {
                operate.Destination = request.Destination;
            }

            if (request.HasDescription)
            {
                operate.Description = request.Description;
            }

            return operate.ToResponse();
        });
    }

    public override Task<CommandOperateResponse> CreateCommandOperate(CommandOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var operate = _provider.GetRequiredService<ICommandOperate>();
            operate.Initialization(request.CommandTemplate);
            _manager.Add(operate);
            operate.Arguments.AddRange(request.Arguments.Select(arg => (CommandOperateArgument)arg));

            return operate.ToResponse();
        });
    }

    public override Task<Empty> RemoveOperate(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.RemoveOperate(request.Guid);
            return new Empty();
        });
    }

    public override Task<Empty> UpdateOperate(OperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (request.HasDescription &&
                _manager.TryGetOperate(request.Guid, out var operate))
            {
                operate.Description = request.Description;
            }

            return new Empty();
        });
    }

    public override Task<Empty> UpdateMoveOperate(UpdateMoveOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetOperate(request.Guid, out var value) &&
                value is IMoveOperate operate)
            {
                operate.Destination = request.Destination;
                if (request.HasDescription)
                {
                    operate.Description = request.Description;
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> UpdateUnpackOperate(UpdateUnpackOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetOperate(request.Guid, out var value) &&
                value is IUnpackOperate operate)
            {
                if (request.HasDestination)
                {
                    operate.Destination = string.IsNullOrEmpty(request.Destination) ? null : request.Destination;
                }

                if (request.HasDescription)
                {
                    operate.Description = request.Description;
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> UpdateCommandOperate(UpdateCommandOperateRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetOperate(request.Guid, out var value) &&
                value is ICommandOperate operate)
            {
                if (request.HasCommandTemplate)
                {
                    operate.CommandLineTemplate = request.CommandTemplate;
                }

                if (request.HasDescription)
                {
                    operate.Description = request.Description;
                }

                if (request.Arguments.Any())
                {
                    operate.Arguments.Clear();
                    operate.Arguments.AddRange(request.Arguments.Select(arg => (CommandOperateArgument)arg));
                }
            }
            return new Empty();
        });
    }

    public override Task<ChainResponse> CreateChain(ChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var chain = _provider.GetRequiredService<IChain>();
            chain.Initialization(request.Name);
            if (request.HasDescription)
            {
                chain.Description = request.Description;
            }
            _manager.Add(chain);

            return chain.ToResponse();
        });
    }

    public override Task<Empty> UpdateChain(UpdateChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetChain(request.Guid, out var chain))
            {
                if (request.HasName)
                {
                    chain.Name = request.Name;
                }

                if (request.HasDescription)
                {
                    chain.Description = request.Description;
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> RemoveChain(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.RemoveChain(request.Guid);
            return new Empty();
        });
    }

    public override Task<Empty> AddOperateTo(OperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddOperateToChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> AddOperateToMany(OperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddOperateToChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> AddManyOperateTo(ManyOperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddOperateToChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> AddManyOperateToMany(ManyOperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddOperateToChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> RemoveOperateFrom(OperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveOperateFromChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> RemoveOperateFromMany(OperateAndManyChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveOperateFromChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> RemoveManyOperateFrom(ManyOperateAndChainRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveOperateFromChain(request);

            return new Empty();
        });
    }

    public override Task<Empty> RemoveManyOperateFromMany(ManyOperateAndManyChainRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveOperateFromChain(request);

            return new Empty();
        });
    }

    public override async Task<Empty> ClearUpAll(Empty request, ServerCallContext context)
    {
        Task[] tasks = { ClearOperates(request, context), ClearChains(request, context) };
        await Task.WhenAll(tasks);

        return new Empty();
    }

    public override Task<Empty> ClearOperates(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.ClearOperates();

            return new Empty();
        });
    }

    public override Task<Empty> ClearChains(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.ClearChains();
            return new Empty();
        });
    }

    public override async Task GetOperates(Empty request, IServerStreamWriter<OperateResponse> responseStream,
        ServerCallContext context)
    {
        foreach (var operate in _manager.Operates)
        {
            await responseStream.WriteAsync(operate.ToResponse());
        }
    }

    public override async Task GetOperatesOf(GuidRequest request, IServerStreamWriter<OperateResponse> responseStream,
        ServerCallContext context)
    {
        if (!_manager.TryGetChain(Guid.Parse((ReadOnlySpan<char>)request.Guid), out var chain))
        {
            return;
        }

        foreach (var operate in chain)
        {
            await responseStream.WriteAsync(operate.ToResponse());
        }
    }

    public override async Task GetChains(Empty request, IServerStreamWriter<ChainResponse> responseStream,
        ServerCallContext context)
    {
        foreach (var chain in _manager.Chains)
        {
            var response = chain.ToResponse();

            response.Operates.AddRange(chain.Select(o => o.ToResponse()));

            await responseStream.WriteAsync(response);
        }
    }

    public override Task<OperateResponse> FindOperate(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetOperate(Guid.Parse(request.Guid), out var operate))
            {
                return operate.ToResponse();
            }

            return new OperateResponse();
        });
    }

    public override Task<ChainResponse> FindChain(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetChain(request, out var chain))
            {
                var result = chain.ToResponse();
                result.Operates.AddRange(chain.Select(o => o.ToResponse()));

                return result;
            }

            return new ChainResponse();
        });
    }

    public override Task<ChainResponse> FindChainWithoutOperates(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetChain(request, out var chain))
            {
                return chain.ToResponse();
            }

            return new ChainResponse();
        });
    }
}