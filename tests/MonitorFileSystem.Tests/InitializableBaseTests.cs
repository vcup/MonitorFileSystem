using System;
using Microsoft.Extensions.DependencyInjection;
using MonitorFileSystem.Common;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public abstract class InitializableBaseTests
{
    protected IServiceProvider Provider = null!;

    [Test]
    public virtual void Initialization_CheckIsInitialization_ThrowExceptionWhenIsInitialized()
    {
        var scope = Provider.CreateScope();

        var initializable = scope.ServiceProvider.GetRequiredService<IInitializable>();
        initializable.Initialization();

        Assert.Throws<InvalidOperationException>(initializable.Initialization, "Instance is not Initialized");
    }
}