using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Extensions;

public static class MonitorExtension
{
    public static IServiceCollection AddWatcherFactory(this IServiceCollection services)
    {
        return services.AddScoped<IWatcher, Watcher>()
            .AddScoped<WatcherFactory>();
    }
}