using Grpc.Net.ClientFactory;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Grpc;

public static class GrpcClientStartup
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddGrpcClient<MonitorManagement.MonitorManagementClient>(Setup);
        services.AddGrpcClient<ActionManagement.ActionManagementClient>(Setup);
        services.AddGrpcClient<MasterManagement.MasterManagementClient>(Setup);

        void Setup(GrpcClientFactoryOptions options)
        {
            options.Address = new Uri("http://localhost:5000");
        }

        return services;
    }
}