using MonitorFileSystem.Action;

namespace MonitorFileSystem.Extensions;

public static class ActionExtension
{
    public static IServiceCollection AddMoveOperate(this IServiceCollection services)
    {
        return services.AddScoped<IMoveOperate, MoveOperate>();
    }

    public static IServiceCollection AddOperates(this IServiceCollection services)
    {
        return services.AddMoveOperate();
    }

    public static IServiceCollection AddChain(this IServiceCollection services)
    {
        return services; //.AddScoped<IChain, Chain>();
    }

    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services.AddChain().AddOperates();
    }
}