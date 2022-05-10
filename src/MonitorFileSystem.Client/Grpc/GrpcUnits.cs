using Grpc.Net.Client;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Grpc;

public static class GrpcUnits
{
    private static GrpcChannel? _grpcChannel;
    static GrpcUnits()
    {
        Settings = new GrpcSettings();
    }
    
    public static GrpcSettings Settings { get; }

    public static GrpcChannel Channel => _grpcChannel ??= GrpcChannel.ForAddress(Settings.Address);

    public static MonitorManagement.MonitorManagementClient MonitorManagementClient => new (Channel);
    public static ActionManagement.ActionManagementClient ActionManagementClient => new (Channel);
    public static MasterManagement.MasterManagementClient MasterManagementClient => new (Channel);
}