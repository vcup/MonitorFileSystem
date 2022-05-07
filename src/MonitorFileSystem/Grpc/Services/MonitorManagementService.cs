using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Grpc.Services;

public class MonitorManagementService : MonitorManagement.MonitorManagementBase
{
    private readonly ILogger<MonitorManagementService> _logger;
    private readonly IMonitorManager _manager;

    public MonitorManagementService(ILogger<MonitorManagementService> logger, IMonitorManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public override Task<WatcherResponse> CreateWatcher(WatcherRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var result = request.ToWatcher();
            _manager.Add(result);
            return result.ToResponse();
        });
    }

    public override Task<Empty> RemoveWatcher(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.RemoveWatcher(Guid.Parse(request.Guid));
            return new Empty();
        });
    }

    public override Task<Empty> DisableWatcher(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetWatcher(request, out var watcher))
            {
                watcher.Monitoring = false;
            }
            return new Empty();
        });
    }

    public override Task<Empty> EnableWatcher(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetWatcher(request, out var watcher))
            {
                watcher.Monitoring = true;
            }
            return new Empty();
        });
    }

    public override Task<Empty> ToggleWatcher(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetWatcher(request, out var watcher))
            {
                watcher.Monitoring = !watcher.Monitoring;
            }
            return new Empty();
        });
    }

    public override Task<Empty> UpdateWatcher(UpdateWatcherRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetWatcher(Guid.Parse(request.Guid), out var watcher))
            {
                watcher.Name = request.Name;
                watcher.MonitorPath = request.Path;
                watcher.Filter = request.Filter;

                if (request.EventCase.HasFlag(UpdateWatcherRequest.EventOneofCase.EventValue))
                {
                    watcher.WatchingEvent = (WatchingEvent)request.EventValue;
                }
            }
            return new Empty();
        });
    }

    public override Task<GroupResponse> CreateGroup(GroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            var result = request.ToGroup();
            _manager.Add(result);
            return result.ToResponse();
        });
    }

    public override Task<Empty> RemoveGroup(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.RemoveGroup(Guid.Parse(request.Guid));
            return new Empty();
        });
    }

    public override Task<Empty> UpdateGroup(UpdateGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetGroup(Guid.Parse(request.Guid), out var group))
            {
                group.Name = request.Name;
                group.Description = request.Description;
            }
            return new Empty();
        });
    }

    public override Task<Empty> AddWatcherTo(WatcherAndGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddWatcherToGroup(request);
            return new Empty();
        });
    }

    public override Task<Empty> AddManyWatcherTo(ManyWatcherAndGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddWatcherToGroup(request);
            return new Empty();
        });
    }

    public override Task<Empty> AddWatcherToMany(WatcherAndManyGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddWatcherToGroup(request);
            return new Empty();
        });
    }
    
    public override Task<Empty> AddManyWatcherToMany(ManyWatcherAndManyGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryAddWatcherToGroup(request);
            return new Empty();
        });
    }

    public override Task<Empty> RemoveWatcherFor(WatcherAndGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveWatcherFromGroup(request);
            return new Empty();
        });
    }
    
    public override Task<Empty> RemoveManyWatcherFor(ManyWatcherAndGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveWatcherFromGroup(request);
            return new Empty();
        });
    }
    
    public override Task<Empty> RemoveWatcherForMany(WatcherAndManyGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveWatcherFromGroup(request);
            return new Empty();
        });
    }

    public override Task<Empty> RemoveManyWatcherForMany(ManyWatcherAndManyGroupRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.TryRemoveWatcherFromGroup(request);
            return new Empty();
        });
    }


    public override Task<Empty> ClearUpAll(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.ClearWatchers();
            _manager.ClearGroups();
            return new Empty();
        });
    }

    public override Task<Empty> ClearWatcher(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.ClearWatchers();
            return new Empty();
        });
    }

    public override Task<Empty> ClearGroup(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _manager.ClearGroups();
            return new Empty();
        });
    }

    public override async Task GetWatchers(Empty request, IServerStreamWriter<WatcherResponse> responseStream, ServerCallContext context)
    {
        foreach (var watcher in _manager.Watchers)
        {
            await responseStream.WriteAsync(watcher.ToResponse());
        }
    }

    public override async Task GetWatchersOf(GuidRequest request, IServerStreamWriter<WatcherResponse> responseStream, ServerCallContext context)
    {
        if (!_manager.TryGetGroup(request, out var group))
        {
            return;
        }
        
        foreach (var watcher in group)
        {
            await responseStream.WriteAsync(watcher.ToResponse());
        }
    }

    public override async Task GetGroups(Empty request, IServerStreamWriter<GroupResponse> responseStream, ServerCallContext context)
    {
        foreach (var group in _manager.Groups)
        {
            await responseStream.WriteAsync(group.ToResponse());
        }
    }

    public override Task<WatcherResponse> FindWatcher(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetWatcher(request, out var watcher))
            {
                return watcher.ToResponse();
            }

            return new WatcherResponse();
        });
    }

    public override Task<GroupResponse> FindGroup(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetGroup(request, out var group))
            {
                var response = group.ToResponse();
                response.Watchers.AddRange(group.Select(w => w.ToResponse()));
                return response;
            }

            return new GroupResponse();
        });
    }

    public override Task<GroupResponse> FindGroupWithoutOperates(GuidRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            if (_manager.TryGetGroup(request, out var group))
            {
                return group.ToResponse();
            }

            return new GroupResponse();
        });
    }
}