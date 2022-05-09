using Grpc.Core;
using Grpc.Net.Client;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client;

public static class GrpcClientStartup
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddScoped<ChannelBase>(provider =>
        {
            var settings = provider.GetRequiredService<GrpcSettings>();
            return GrpcChannel.ForAddress(settings.Address);
        });
        services.AddTransient<MonitorManagement.MonitorManagementClient>();
        services.AddTransient<ActionManagement.ActionManagementClient>();
        services.AddTransient<MasterManagement.MasterManagementClient>();
        
        return services;
    }
}