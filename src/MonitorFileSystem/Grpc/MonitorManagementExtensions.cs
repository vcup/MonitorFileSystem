using System.Diagnostics.CodeAnalysis;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Grpc;

public static class MonitorManagementExtensions
{
    public static IWatcher ToWatcher(this WatcherRequest request)
    {
        var result = new Watcher(request.Name, request.Path, request.Filter);

        if (request.HasEvent)
        {
            result.WatchingEvent = (WatchingEvent)request.Event;
        }

        if (request.HasEventFlags)
        {
            result.WatchingEvent = (WatchingEvent)request.EventFlags;
        }
        
        return result;
    }

    public static IGroup ToGroup(this GroupRequest request)
    {
        return new Group(request.Name, request.Description);
    }

    public static bool TryGetWatcher(this IMonitorManager manager, GuidRequest request, [MaybeNullWhen(false)] out IWatcher watcher)
    {
        return manager.TryGetWatcher(Guid.Parse(request.Guid), out watcher);
    }

    public static bool TryGetWatcher(this IMonitorManager manager, UpdateWatcherEventRequest request,
        [MaybeNullWhen(false)] out IWatcher watcher)
    {
        return manager.TryGetWatcher(Guid.Parse(request.Guid), out watcher);
    }
    
    public static bool TryGetGroup(this IMonitorManager manager, GuidRequest request, [MaybeNullWhen(false)] out IGroup group)
    {
        return manager.TryGetGroup(Guid.Parse(request.Guid), out group);
    }

    public static bool TryAddWatcherToGroup(this IMonitorManager manager, WatcherAndGroupRequest request)
    {
        return manager.TryAddWatcherToGroup(Guid.Parse(request.Watcher.Guid), Guid.Parse(request.Group.Guid));
    }
    
    public static bool TryAddWatcherToGroup(this IMonitorManager manager, WatcherAndManyGroupRequest request)
    {
        var operateGuid = Guid.Parse(request.Watcher.Guid);
    
        return request.Groups.All(chain => manager.TryAddWatcherToGroup(operateGuid, Guid.Parse(chain.Guid)));
    }
    
    public static bool TryAddWatcherToGroup(this IMonitorManager manager, ManyWatcherAndGroupRequest request)
    {
        var chainGuid = Guid.Parse(request.Group.Guid);
        
        return request.Watchers.All(operate => manager.TryAddWatcherToGroup(Guid.Parse(operate.Guid), chainGuid));
    }
    
    public static bool TryAddWatcherToGroup(this IMonitorManager manager, ManyWatcherAndManyGroupRequest request)
    {
        var operateGuids = request.Watchers.Select(operate => Guid.Parse(operate.Guid));
        var chainGuids = request.Groups.Select(chain => Guid.Parse(chain.Guid));
    
        return operateGuids.Zip(chainGuids)
            .All(union => manager.TryAddWatcherToGroup(union.First, union.Second));
    }
    
    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, WatcherAndGroupRequest request)
    {
        return manager.TryRemoveWatcherFromGroup(Guid.Parse(request.Watcher.Guid), Guid.Parse(request.Group.Guid));
    }
    
    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, WatcherAndManyGroupRequest request)
    {
        var operateGuid = Guid.Parse(request.Watcher.Guid);
    
        return request.Groups.All(chain => manager.TryRemoveWatcherFromGroup(operateGuid, Guid.Parse(chain.Guid)));
    }
    
    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, ManyWatcherAndGroupRequest request)
    {
        var chainGuid = Guid.Parse(request.Group.Guid);
        
        return request.Watchers.All(operate => manager.TryRemoveWatcherFromGroup(Guid.Parse(operate.Guid), chainGuid));
    }
    
    public static bool TryRemoveWatcherFromGroup(this IMonitorManager manager, ManyWatcherAndManyGroupRequest request)
    {
        var operateGuids = request.Watchers.Select(operate => Guid.Parse(operate.Guid));
        var chainGuids = request.Groups.Select(chain => Guid.Parse(chain.Guid));
    
        return operateGuids.Zip(chainGuids)
            .All(union => manager.TryRemoveWatcherFromGroup(union.First, union.Second));
    }

    public static WatcherResponse ToResponse(this IWatcher watcher)
    {
        return new WatcherResponse
        {
            Guid = watcher.Guid.ToString(),
            Name = watcher.Name,
            Path = watcher.MonitorPath,
            Filter = watcher.Filter,
            Event = (int)watcher.WatchingEvent,
            IsEnable = watcher.Monitoring,
        };
    }

    public static GroupResponse ToResponse(this IGroup group)
    {
        return new GroupResponse
        {
            Guid = group.Guid.ToString(),
            Name = group.Name,
            Description = group.Description,
        };
    }
}