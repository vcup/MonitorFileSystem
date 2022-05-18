using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Grpc.Services;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Grpc;

public class GrpcStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IMonitorManager, MonitorManager>()
            .AddSingleton<IActionManager, ActionManager>()
            .AddMoveOperate()
            .AddUnpackOperate()
            .AddChain()
            .AddGrpc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<MonitorManagementService>();
            endpoints.MapGrpcService<ActionManagementService>();
            endpoints.MapGrpcService<MasterManagementService>();
        });
    }
}