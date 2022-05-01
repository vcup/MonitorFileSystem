using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public class MoveOperateTests
{
    private IServiceProvider _provider = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _provider = Host.CreateDefaultBuilder()
            .ConfigureServices((services) =>
            {
                services.AddMoveOperate();
            })
            .Build()
            .Services;
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ProcessFileBranch_InvalidDestinationPath_ThrowDirectoryNotFoundException()
    {
        var files = new Dictionary<string, MockFileData>
        {
            {"/file", MockFileData.NullObject}
        };
        var filesystem = new MockFileSystem(files);
        var operate = _provider.GetService<MoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization(filesystem, "/InValidPath/file");
        var info = new WatchingEventInfo
        {
            Path = "/file"
        };

        Assert.Throws<DirectoryNotFoundException>(() => operate.Process(info));
    }

    [Test]
    public void ProcessDirectoryBranch_WatchedEventInfo_UpdatePathToMovedPath()
    {
        var files = new Dictionary<string, MockFileData>
        {
            { "/directory_1", new MockDirectoryData() },
            { "/directory_2", new MockDirectoryData() }
        };
        var filesystem = new MockFileSystem(files);
        var operate = _provider.GetService<MoveOperate>();
        Assert.IsNotNull(operate);
        operate!.Initialization(filesystem, "/directory_2");
        var info = new WatchingEventInfo
        {
            Path = "/directory_1"
        };
        
        operate.Process(info);
        
        Assert.AreEqual("/directory_2/directory_1", info.Path);
    }

}