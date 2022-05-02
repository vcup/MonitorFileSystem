using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
}