using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Action;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Grpc.Services;

public class MasterManagementService : MasterManagement.MasterManagementBase
{
    private readonly ILogger<MasterManagementService> _logger;
    private readonly IMonitorManager _monitor;
    private readonly IActionManager _action;

    private static readonly Dictionary<(Guid, Guid), IDisposable> Disposables = new();

    public MasterManagementService(ILogger<MasterManagementService> logger, IMonitorManager monitor,
        IActionManager action)
    {
        _logger = logger;
        _monitor = monitor;
        _action = action;
    }

    public override Task<Empty> AttachActionToMonitor(MonitorAndActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var monitorGuid = request.GetMonitorGuid();
            var actionGuid = request.GetActionGuid();

            if (_monitor.TryGetObservable(monitorGuid, out var monitor) &&
                _action.TryGetObserver(actionGuid, out var action))
            {
                Disposables.Add((monitorGuid, actionGuid), monitor.Subscribe(action));
            }

            return new Empty();
        });
    }

    public override Task<Empty> AttachActionToManyMonitor(ManyMonitorAndActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var actionGuid = request.GetActionGuid();

            if (_action.TryGetObserver(actionGuid, out IObserver<WatchingEventInfo>? action))
            {
                foreach (var monitorGuid in request.Monitors.Select(monitor => Guid.Parse(monitor.Guid)))
                {
                    if (_monitor.TryGetObservable(monitorGuid, out IObservable<WatchingEventInfo>? monitor))
                    {
                        Disposables.Add(ValueTuple.Create(monitorGuid, actionGuid), monitor.Subscribe(action));
                    }
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> AttachManyActionToMonitor(MonitorAndManyActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var monitorGuid = request.GetMonitorGuid();
            if (_monitor.TryGetObservable(monitorGuid, out IObservable<WatchingEventInfo>? monitor))
            {
                foreach (var actionGuid in request.Actions.Select(action => Guid.Parse(action.Guid)))
                {
                    if (_action.TryGetObserver(actionGuid, out IObserver<WatchingEventInfo>? action))
                    {
                        Disposables.Add(ValueTuple.Create(monitorGuid, actionGuid), monitor.Subscribe(action));
                    }
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> AttachManyActionToManyMonitor(ManyMonitorAndManyActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var guids in request.GetBothGuids())
            {
                if (_monitor.TryGetObservable(guids.Item1, out IObservable<WatchingEventInfo>? monitor) &&
                    _action.TryGetObserver(guids.Item2, out IObserver<WatchingEventInfo>? action))
                {
                    Disposables.Add(guids, monitor.Subscribe(action));
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> DetachActionToMonitor(MonitorAndActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var key = (request.GetMonitorGuid(), request.GetActionGuid());
            if (Disposables.TryGetValue(key, out var value))
            {
                value.Dispose();
                Disposables.Remove(key);
            }

            return new Empty();
        });
    }

    public override Task<Empty> DetachActionToManyMonitor(ManyMonitorAndActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var actionGuid = request.GetActionGuid();
            foreach (var monitorGuid in request.Monitors.Select(monitor => Guid.Parse(monitor.Guid)))
            {
                var key = (monitorGuid, actionGuid);
                if (Disposables.TryGetValue(key, out var value))
                {
                    value.Dispose();
                    Disposables.Remove(key);
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> DetachManyActionToMonitor(MonitorAndManyActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var monitorGuid = request.GetMonitorGuid();
            foreach (var actionGuid in request.Actions.Select(action => Guid.Parse(action.Guid)))
            {
                var key = (monitorGuid, actionGuid);
                if (Disposables.TryGetValue(key, out var value))
                {
                    value.Dispose();
                    Disposables.Remove(key);
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> DetachManyActionToManyMonitor(ManyMonitorAndManyActionRequest request,
        ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var key in request.GetBothGuids())
            {
                if (Disposables.TryGetValue(key, out var value))
                {
                    value.Dispose();
                    Disposables.Remove(key);
                }
            }

            return new Empty();
        });
    }


    public override async Task GetActions(Empty request, IServerStreamWriter<GuidResponse> responseStream,
        ServerCallContext context)
    {
        var actionGuids = Disposables.Keys
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetMonitors(Empty request, IServerStreamWriter<GuidResponse> responseStream,
        ServerCallContext context)
    {
        var monitorGuids = Disposables.Keys
            .Select(guids => guids.Item1);

        foreach (var guid in monitorGuids)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetRelations(Empty request, IServerStreamWriter<MonitorAndActionResponse> responseStream,
        ServerCallContext context)
    {
        foreach (var (monitorGuid, actionGuid) in Disposables.Keys)
        {
            await responseStream.WriteAsync(new MonitorAndActionResponse
            {
                Monitor = monitorGuid.ToResponse(),
                Action = actionGuid.ToResponse()
            });
        }
    }

    public override async Task GetRelationOfAction(GuidRequest request,
        IServerStreamWriter<GuidResponse> responseStream, ServerCallContext context)
    {
        var results = Disposables.Keys
            .Select(guids => guids.Item2)
            .Where(guid => guid == request.GetGuid());
        foreach (var guid in results)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetRelationOfMonitor(GuidRequest request,
        IServerStreamWriter<GuidResponse> responseStream, ServerCallContext context)
    {
        var results = Disposables.Keys
            .Select(guids => guids.Item1)
            .Where(guid => guid == request.GetGuid());
        foreach (var guid in results)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetRelationOfEither(GuidRequest request,
        IServerStreamWriter<GuidResponse> responseStream, ServerCallContext context)
    {
        var guid = request.GetGuid();
        if (_monitor.TryGetObservable(guid, out _))
        {
            await GetRelationOfMonitor(request, responseStream, context);
        }
        else if (_action.TryGetOperate(guid, out _))
        {
            await GetRelationOfAction(request, responseStream, context);
        }
    }


    public override async Task ActionAttachedMonitors(GuidRequest request,
        IServerStreamWriter<GuidResponse> responseStream, ServerCallContext context)
    {
        var monitors = Disposables.Keys
            .Where(guids => guids.Item2 == request.GetGuid())
            .Select(guids => guids.Item1);

        foreach (var guid in monitors)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task ManyActionAttachedMonitors(IAsyncStreamReader<GuidRequest> requestStream,
        IServerStreamWriter<GuidResponse> responseStream,
        ServerCallContext context)
    {
        var actionGuid = requestStream.Current.GetGuid();
        var monitors = Disposables.Keys
            .Where(guids => guids.Item2 == actionGuid)
            .Select(guids => guids.Item1);

        foreach (var guid in monitors)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task ActionsOnMonitor(GuidRequest request, IServerStreamWriter<GuidResponse> responseStream,
        ServerCallContext context)
    {
        var monitorGuid = request.GetGuid();
        var actionGuids = Disposables.Keys
            .Where(guids => guids.Item1 == monitorGuid)
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task ActionsOnManyMonitors(IAsyncStreamReader<GuidRequest> requestStream,
        IServerStreamWriter<GuidResponse> responseStream, ServerCallContext context)
    {
        var monitorGuid = requestStream.Current.GetGuid();
        var actionGuids = Disposables.Keys
            .Where(guids => guids.Item1 == monitorGuid)
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override Task<Empty> Clear(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var value in Disposables.Values)
            {
                value.Dispose();
            }

            Disposables.Clear();

            return new Empty();
        });
    }
}