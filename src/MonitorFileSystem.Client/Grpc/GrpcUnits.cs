using Grpc.Net.Client;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Grpc;

public static class GrpcUnits
{
    static GrpcUnits()
    {
        Settings = new GrpcSettings();
    }
    
    public static GrpcSettings Settings { get; }

    public static Lazy<GrpcChannel> Channel => new(GrpcChannel.ForAddress(Settings.Address));

    public static MonitorManagement.MonitorManagementClient MonitorManagementClient => new (Channel.Value);
    public static ActionManagement.ActionManagementClient ActionManagementClient => new (Channel.Value);
    public static MasterManagement.MasterManagementClient MasterManagementClient => new (Channel.Value);
}