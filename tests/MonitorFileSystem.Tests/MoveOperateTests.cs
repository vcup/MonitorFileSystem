using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public class MoveOperateTests : OperateBaseTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Provider = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddMoveOperate()
                    .AddScoped<IFileSystem, MockFileSystem>()
                    .AddScoped<IOperate, MoveOperate>();
            })
            .Build()
            .Services;
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public override void Process_CheckIsInitialization_ThrowExceptionWhenIsInitialized()
    {
        var scope = Provider.CreateScope();

        var dest = "./";
        var operate = scope.ServiceProvider.GetService<IMoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization(dest);

        Assert.Throws<InvalidOperationException>(() => operate.Initialization(dest), "Instance is not Initialized");
    }

    [Test]
    public void ProcessFileBranch_InvalidDestinationPath_ThrowDirectoryNotFoundException()
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        filesystem!.AddFile("/file", new MockFileData(string.Empty));

        var operate = scope.ServiceProvider.GetService<IMoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization("/InValidPath/file");
        var info = new WatchingEventInfo
        {
            Path = "/file"
        };

        Assert.Throws<DirectoryNotFoundException>(() => operate.Process(info));
    }

    [Test]
    public void ProcessDirectoryBranch_WatchedEventInfo_UpdatePathToMovedPath()
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        filesystem!.AddDirectory("/directory_1");
        filesystem.AddDirectory("/directory_2");

        var operate = scope.ServiceProvider.GetService<IMoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization("/directory_2");
        var info = new WatchingEventInfo
        {
            Path = "/directory_1"
        };

        operate.Process(info);

        Assert.AreEqual("/directory_2/directory_1", info.Path.Replace('\\', '/'));
    }

    [TestCase("/d1", "/d2", "/d2/d1")]
    [TestCase("/d1", "/f/d2", "/f/d2/d1")]
    [TestCase("/f1/d1", "/f2/d2", "/f2/d2/d1")]
    [TestCase("/f/d1", "/f/d2", "/f/d2/d1")]
    [TestCase("/d1/", "/d2", "/d2/d1")]
    [TestCase("/f1/d1/", "/f2/d2", "/f2/d2/d1")]
    [TestCase("/f/d1/", "/f/d2", "/f/d2/d1")]
    public void ProcessDirectoryBranch_ComplexStruct_Intuitive(string path1, string path2, string result)
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        filesystem!.AddDirectory(path1);
        filesystem.AddDirectory(path2);

        var operate = scope.ServiceProvider.GetRequiredService<IMoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization(path2);
        var info = new WatchingEventInfo
        {
            Path = path1
        };

        operate.Process(info);

        Assert.AreEqual(result, info.Path.Replace('\\', '/'));
    }

    [Test]
    public void Initialization_NonParameterInitialization_MustThrowNotImplementedException()
    {
        var scope = Provider.CreateScope();

        var operate = scope.ServiceProvider.GetService<IMoveOperate>();
        Assert.IsNotNull(operate);

        Assert.Throws<NotImplementedException>(() => operate!.Initialization());
    }
}