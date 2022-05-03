using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Grpc;

public static class MonitorManageExtensions
{
    public static IWatcher ToWatcher(this WatcherRequest req)
    {
        switch (req.EventCase)
        {
            case WatcherRequest.EventOneofCase.None:
                return new Watcher(req.Name, req.Path, req.Filter);
            case WatcherRequest.EventOneofCase.EventValue:
                return new Watcher(req.Name, req.Path, req.Filter, (WatchingEvent)req.EventValue);
            default:
                throw new ArgumentException("{Name} has unexpected .EventCase", nameof(req));
        }
    }

    public static IWatcher ToWatcher(this WatcherRequest req, IMonitorManager manager)
    {
        var watcher = manager.FindWatcher(req.Name);

        if (watcher is null)
        {
            manager.Add(watcher = req.ToWatcher());
        }

        return watcher;
    }

    public static IGroup ToGroup(this GroupRequest req)
    {
        return new Group(req.Name, req.Description);
    }

    public static IGroup ToGroup(this GroupRequest req, IMonitorManager manager)
    {
        var group = manager.FindGroup(req.Name);
        
        if (group is null)
        {
            manager.Add(group = req.ToGroup());
        }

        return group;
    }
}