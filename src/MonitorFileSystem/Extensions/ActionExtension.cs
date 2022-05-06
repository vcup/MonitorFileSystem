﻿using System.IO.Abstractions;
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

    public static IServiceCollection AddOperates(this IServiceCollection services)
    {
        return services.AddMoveOperate()
            .AddUnpackOperate()
            ;
    }

    public static IServiceCollection AddChain(this IServiceCollection services)
    {
        return services.AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IChain, Chain>();
    }

    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services.AddChain().AddOperates();
    }
}