using System;
using Microsoft.Extensions.DependencyInjection;
using MonitorFileSystem.Action;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public abstract class OperateBaseTests
{
    protected IServiceProvider Provider = null!;
    
    [Test]
    public void Process_CheckIsInitialization_ThrowExceptionWhenIsNotInitialized()
    {
        var scope = Provider.CreateScope();

        var info = new WatchingEventInfo();
        
        var operate = scope.ServiceProvider.GetService<IOperate>();
        Assert.IsNotNull(operate);

        Assert.Throws<InvalidOperationException>(() => operate!.Process(info), "Instance is already Initialized");
    }
    
    [Test]
    public virtual void Process_CheckIsInitialization_ThrowExceptionWhenIsInitialized()
    {
        var scope = Provider.CreateScope();
        
        var operate = scope.ServiceProvider.GetService<IOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization();
    
        Assert.Throws<InvalidOperationException>(operate.Initialization, "Instance is not Initialized");
    }
}