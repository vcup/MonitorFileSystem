using Grpc.Net.Client;
using MonitorFileSystem.Client.Configures;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client;

internal static class GrpcUnits
{
    private static GrpcChannel? _grpcChannel;

    private static GrpcChannel Channel => _grpcChannel ??= GrpcChannel.ForAddress(Configure.GrpcSettings.Address);

    public static MonitorManagement.MonitorManagementClient MonitorManagementClient => new(Channel);
    public static ActionManagement.ActionManagementClient ActionManagementClient => new(Channel);
    public static MasterManagement.MasterManagementClient MasterManagementClient => new(Channel);
}