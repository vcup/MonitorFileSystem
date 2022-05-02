using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MonitorFileSystem.Grpc.Services;

namespace MonitorFileSystem.Grpc;

public class GrpcStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
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
        });
    }
}