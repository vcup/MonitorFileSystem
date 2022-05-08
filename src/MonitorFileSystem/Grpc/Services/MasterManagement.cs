﻿using Google.Protobuf.WellKnownTypes;
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

    private readonly Dictionary<(Guid, Guid), IDisposable> _disposables = new();

    public MasterManagementService(ILogger<MasterManagementService> logger, IMonitorManager monitor, IActionManager action)
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
            
            if (_monitor.TryGetObservable(monitorGuid, out IObservable<WatchingEventInfo>? monitor) &&
                _action.TryGetObserver(actionGuid, out IObserver<WatchingEventInfo>? action))
            {
                _disposables.Add(ValueTuple.Create(monitorGuid, actionGuid), monitor.Subscribe(action));
            }

            return new Empty();
        });
    }

    public override Task<Empty> AttachActionToManyMonitor(ManyMonitorAndActionRequest request, ServerCallContext context)
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
                        _disposables.Add(ValueTuple.Create(monitorGuid, actionGuid), monitor.Subscribe(action));
                    }
                }
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> AttachManyActionToMonitor(MonitorAndManyActionRequest request, ServerCallContext context)
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
                        _disposables.Add(ValueTuple.Create(monitorGuid, actionGuid), monitor.Subscribe(action));
                    }
                }
            }

            return new Empty();
        });
    }

    public override Task<Empty> AttachManyActionToManyMonitor(ManyMonitorAndManyActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var guids in request.GetBothGuids())
            {
                if (_monitor.TryGetObservable(guids.Item1, out IObservable<WatchingEventInfo>? monitor) &&
                    _action.TryGetObserver(guids.Item2, out IObserver<WatchingEventInfo>? action))
                { 
                    _disposables.Add(guids, monitor.Subscribe(action));
                }
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> DetachActionToMonitor(MonitorAndActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_disposables.TryGetValue(ValueTuple.Create(request.GetMonitorGuid(), request.GetActionGuid()),
                    out var value))
            {
                value.Dispose();
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> DetachActionToManyMonitor(ManyMonitorAndActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var actionGuid = request.GetActionGuid();
            foreach (var monitorGuid in request.Monitors.Select(monitor => Guid.Parse(monitor.Guid)))
            {
                if (_disposables.TryGetValue(ValueTuple.Create(monitorGuid, actionGuid),
                        out var value))
                {
                    value.Dispose();
                }
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> DetachManyActionToMonitor(MonitorAndManyActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var monitorGuid = request.GetMonitorGuid();
            foreach (var actionGuid in request.Actions.Select(action => Guid.Parse(action.Guid)))
            {
                if (_disposables.TryGetValue(ValueTuple.Create(monitorGuid, actionGuid),
                        out var value))
                {
                    value.Dispose();
                }                
            }
            
            return new Empty();
        });
    }

    public override Task<Empty> DetachManyActionToManyMonitor(ManyMonitorAndManyActionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var guids in request.GetBothGuids())
            {
                if (_disposables.TryGetValue(guids, out var value))
                {
                    value.Dispose();
                }
            }
            
            return new Empty();
        });
    }


    public override async Task GetActions(Empty request, IServerStreamWriter<GuidRequest> responseStream, ServerCallContext context)
    {
        var actionGuids = _disposables.Keys
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        { 
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetMonitors(Empty request, IServerStreamWriter<GuidRequest> responseStream, ServerCallContext context)
    {
        var monitorGuids = _disposables.Keys
            .Select(guids => guids.Item1);

        foreach (var guid in monitorGuids)
        {
            await responseStream.WriteAsync(guid.ToResponse());
        }
    }

    public override async Task GetMappedActionAndMonitor(Empty request, IServerStreamWriter<MonitorAndActionRequest> responseStream, ServerCallContext context)
    {
        foreach (var (monitorGuid, actionGuid) in _disposables.Keys)
        {
            await responseStream.WriteAsync(new MonitorAndActionRequest
            {
                Monitor = monitorGuid.ToResponse(),
                Action = actionGuid.ToResponse()
            });
        }
    }


    public override async Task ActionAttachedMonitors(GuidRequest request, IServerStreamWriter<GuidRequest> responseStream, ServerCallContext context)
    {
        var monitors = _disposables.Keys
            .Where(guids => guids.Item2 == request.GetGuid())
            .Select(guids => guids.Item1);

        foreach (var guid in monitors)
        {
            await responseStream.WriteAsync(new GuidRequest { Guid = guid.ToString() });
        }
    }

    public override async Task ManyActionAttachedMonitors(IAsyncStreamReader<GuidRequest> requestStream, IServerStreamWriter<GuidRequest> responseStream,
        ServerCallContext context)
    {
        var actionGuid = requestStream.Current.GetGuid();
        var monitors = _disposables.Keys
            .Where(guids => guids.Item2 == actionGuid)
            .Select(guids => guids.Item1);

        foreach (var guid in monitors)
        {
            await responseStream.WriteAsync(new GuidRequest { Guid = guid.ToString() });
        }
    }

    public override async Task ActionsOnMonitor(GuidRequest request, IServerStreamWriter<GuidRequest> responseStream, ServerCallContext context)
    {
        var monitorGuid = request.GetGuid();
        var actionGuids = _disposables.Keys
            .Where(guids => guids.Item1 == monitorGuid)
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        {
            await responseStream.WriteAsync(new GuidRequest { Guid = guid.ToString() });
        }
    }

    public override async Task ActionsOnManyMonitors(IAsyncStreamReader<GuidRequest> requestStream, IServerStreamWriter<GuidRequest> responseStream,
        ServerCallContext context)
    {
        var monitorGuid = requestStream.Current.GetGuid();
        var actionGuids = _disposables.Keys
            .Where(guids => guids.Item1 == monitorGuid)
            .Select(guids => guids.Item2);

        foreach (var guid in actionGuids)
        {
            await responseStream.WriteAsync(new GuidRequest { Guid = guid.ToString() });
        }
    }

    public override Task<Empty> Clear(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            foreach (var value in _disposables.Values)
            {
                value.Dispose();
            }
            _disposables.Clear();

            return new Empty();
        });
    }
}