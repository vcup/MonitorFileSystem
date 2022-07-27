using System.IO.Abstractions;
using MonitorFileSystem.Action;

namespace MonitorFileSystem.Extensions;

public static class ActionExtension
{
    public static IServiceCollection AddMoveOperate(this IServiceCollection services)
    {
        return services.AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IMoveOperate, MoveOperate>();
    }

    public static IServiceCollection AddUnpackOperate(this IServiceCollection services)
    {
        return services.AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IUnpackOperate, UnpackOperate>();
    }

    public static IServiceCollection AddCommandOperate(this IServiceCollection services)
    {
        return services.AddScoped<IFileSystem, FileSystem>()
            .AddScoped<ICommandOperate, CommandOperate>();
    }

    public static IServiceCollection AddChain(this IServiceCollection services)
    {
        return services.AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IChain, Chain>();
    }
}