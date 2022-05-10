using Grpc.Net.Client;

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
}