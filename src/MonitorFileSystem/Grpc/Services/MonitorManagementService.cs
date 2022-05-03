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

    public override Task<Empty> CreateWatcher(WatcherRequest request, ServerCallContext context)
    {
        IWatcher watcher;
        if (request.EventCase.HasFlag(WatcherRequest.EventOneofCase.EventValue))
        {
            watcher = new Watcher(request.Name, request.Path, request.Filter, (WatchingEvent)request.EventValue);
            
            _logger.LogInformation("Created Watcher({Name}) with Event", watcher.Name);
        }
        else
        {
            watcher = new Watcher(request.Name, request.Path, request.Filter);
            _logger.LogInformation("Created Watcher({Name}) without Event", watcher.Name);
        }
        _logger.LogTrace("{Name} - Path: {Path} Filter: {Filter} Event: {Event}",
            watcher.Name, watcher.MonitorPath, watcher.Filter, watcher.WatchingEvent);
        
        _manager.Add(watcher);

        return Task.FromResult(new Empty());
    }

    public override Task<Empty> RemoveWatcher(WatcherRequest request, ServerCallContext context)
    {
        _manager.Remove(request.ToWatcher());
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> DisableWatcher(WatcherRequest request, ServerCallContext context)
    {
        request.ToWatcher(_manager).Monitoring = false;
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> EnableWatcher(WatcherRequest request, ServerCallContext context)
    {
        request.ToWatcher(_manager).Monitoring = true;
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> ToggleWatcher(WatcherRequest request, ServerCallContext context)
    {
        var watcher = request.ToWatcher(_manager);
        watcher.Monitoring = !watcher.Monitoring;
        return Task.FromResult(new Empty());
    }

    
    public override Task<Empty> CreateGroup(GroupRequest request, ServerCallContext context)
    {
        _manager.Add(request.ToGroup());
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> RemoveGroup(GroupRequest request, ServerCallContext context)
    {
        _manager.Remove(request.ToGroup());
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> AddWatcherTo(WatcherAndGroupRequest request, ServerCallContext context)
    {
        request.Group.ToGroup(_manager).Add(request.Watcher.ToWatcher(_manager));
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> AddManyWatcherTo(ManyWatcherAndGroupRequest request, ServerCallContext context)
    {
        foreach (var watcher in request.Watchers)
        {
            request.Group.ToGroup(_manager).Add(watcher.ToWatcher(_manager));
        }
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> AddWatcherToMany(WatcherAndManyGroupRequest request, ServerCallContext context)
    {
        foreach (var group in request.Groups)
        {
            group.ToGroup(_manager).Add(request.Watcher.ToWatcher(_manager));
        }
        return Task.FromResult(new Empty());
    }
    
    public override Task<Empty> RemoveWatcherFor(WatcherAndGroupRequest request, ServerCallContext context)
    {
        request.Group.ToGroup(_manager).Remove(request.Watcher.ToWatcher(_manager));
        return Task.FromResult(new Empty());
    }
    
    public override Task<Empty> RemoveManyWatcherFor(ManyWatcherAndGroupRequest request, ServerCallContext context)
    {
        foreach (var watcher in request.Watchers)
        {
            request.Group.ToGroup(_manager).Remove(watcher.ToWatcher(_manager));
        }
        return Task.FromResult(new Empty());
    }
    
    public override Task<Empty> RemoveWatcherForMany(WatcherAndManyGroupRequest request, ServerCallContext context)
    {
        foreach (var group in request.Groups)
        {
            group.ToGroup(_manager).Remove(request.Watcher.ToWatcher(_manager));
        }
        return Task.FromResult(new Empty());
    }


    public override Task<Empty> ClearUpAll(Empty request, ServerCallContext context)
    {
        _manager.ClearUp();
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> ClearWatcher(Empty request, ServerCallContext context)
    {
        (_manager as ICollection<IWatcher>).Clear();
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> ClearGroup(Empty request, ServerCallContext context)
    {
        (_manager as ICollection<IGroup>).Clear();
        return Task.FromResult(new Empty());
    }
}