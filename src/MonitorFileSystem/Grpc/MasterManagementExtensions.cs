using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Grpc;

public static class MasterManagementExtensions
{
    public static Guid GetActionGuid(this MonitorAndActionRequest request)
    {
        return Guid.Parse(request.Action.Guid);
    }

    public static Guid GetActionGuid(this ManyMonitorAndActionRequest request)
    {
        return Guid.Parse(request.Action.Guid);
    }

    public static Guid GetMonitorGuid(this MonitorAndActionRequest request)
    {
        return Guid.Parse(request.Monitor.Guid);
    }

    public static Guid GetMonitorGuid(this MonitorAndManyActionRequest request)
    {
        return Guid.Parse(request.Monitor.Guid);
    }

    public static IEnumerable<(Guid, Guid)> GetBothGuids(this ManyMonitorAndManyActionRequest request)
    {
        var monitorGuids = request.Monitors.Select(monitor => Guid.Parse(monitor.Guid)); 
        var actionGuids = request.Actions.Select(action => Guid.Parse(action.Guid)); 
        return monitorGuids.Zip(actionGuids);
    }

    public static Guid GetGuid(this GuidRequest request)
    {
        return Guid.Parse(request.Guid);
    }

    public static GuidRequest ToResponse(this Guid guid)
    {
        return new GuidRequest { Guid = guid.ToString() };
    }
}