using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public class OperateBaseTests
{
    protected IServiceProvider Provider = null!;
    
    [OneTimeSetUp]
    public virtual void OneTimeSetup()
    {
        Provider = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<IFileSystem, MockFileSystem>()
                    .AddScoped<IOperate, OperateBase>()
                    ;
            })
            .Build()
            .Services;
    }
    
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
    public void Process_CheckIsInitialization_ThrowExceptionWhenIsInitialized()
    {
        var scope = Provider.CreateScope();
        
        var operate = scope.ServiceProvider.GetService<IOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization();
    
        Assert.Throws<InvalidOperationException>(operate.Initialization, "Instance is not Initialized");
    }
}