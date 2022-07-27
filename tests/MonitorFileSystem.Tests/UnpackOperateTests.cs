using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Common;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public class UnpackOperateTests : InitializableBaseTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Provider = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddUnpackOperate()
                    .AddScoped<IFileSystem, MockFileSystem>()
                    .AddScoped<IInitializable, UnpackOperate>();
            })
            .Build()
            .Services;
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Process_CheckIsInitialization_ThrowExceptionWhenIsNotInitialized()
    {
        var scope = Provider.CreateScope();

        var info = new WatchingEventInfo();

        var operate = scope.ServiceProvider.GetRequiredService<IUnpackOperate>();

        Assert.Throws<InvalidOperationException>(() => operate!.Process(info), "Instance is already Initialized");
    }
    
    [Test]
    public void Process_PathDetect_NotThrowWhenPathIsDirectory()
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetRequiredService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        filesystem!.AddDirectory("/directory_1");
        var operate = scope.ServiceProvider.GetRequiredService<IUnpackOperate>();
        operate.Initialization();
        var info = new WatchingEventInfo
        {
            Path = "/directory_1"
        };

        operate.Process(info);

        Assert.Pass();
    }

    [Test]
    public void Process_UnpackZipArchiveWithSingleFile_UnpackToCurrentPath()
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetRequiredService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        using (var stream = filesystem!.FileStream.Create("./pack.zip", FileMode.Create, FileAccess.ReadWrite))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false))
        {
            zipArchive.CreateEntry("./file");
        }

        var file = new WatchingEventInfo
        {
            Path = "./pack.zip"
        };

        var operate = scope.ServiceProvider.GetRequiredService<IUnpackOperate>();
        operate.Initialization();

        operate.Process(file);

        Assert.IsTrue(filesystem.File.Exists("file"));
    }

    [Test]
    public void Process_UnpackZipArchiveWithMultiFile_UnpackToCurrentPathWithADirectory()
    {
        var scope = Provider.CreateScope();

        var filesystem = scope.ServiceProvider.GetService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        using (var stream = filesystem!.FileStream.Create("./pack.zip", FileMode.Create, FileAccess.ReadWrite))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false))
        {
            zipArchive.CreateEntry("./file_1");
            zipArchive.CreateEntry("./directory_1/");
            zipArchive.CreateEntry("./directory_2/file_2");
        }

        var operate = scope.ServiceProvider.GetRequiredService<IUnpackOperate>();
        operate.Initialization();

        var info = new WatchingEventInfo
        {
            Path = "./pack.zip"
        };

        operate.Process(info);

        Assert.IsTrue(filesystem.File.Exists("./pack/file_1"));
        Assert.IsTrue(filesystem.Directory.Exists("./pack/directory_1"));
        Assert.IsTrue(filesystem.Directory.Exists("./pack/directory_2"));
        Assert.IsTrue(filesystem.File.Exists("./pack/directory_2/file_2"));
    }

    [Test]
    public void Process_UnpackZipWithSameNameFile()
    {
        var scoped = Provider.CreateScope();

        var filesystem = scoped.ServiceProvider.GetRequiredService<IFileSystem>() as MockFileSystem;
        Assert.IsNotNull(filesystem);
        using (var stream = filesystem!.FileStream.Create("./pack.zip", FileMode.CreateNew, FileAccess.ReadWrite))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false))
        {
            var entry = zipArchive.CreateEntry("./pack.zip");
            using var file = entry.Open();
            using var fileWriter = new StreamWriter(file);
            fileWriter.Write("content");
        }

        var operate = scoped.ServiceProvider.GetRequiredService<IUnpackOperate>();
        operate.Initialization();

        var info = new WatchingEventInfo
        {
            Path = "./pack.zip"
        };

        operate.Process(info);

        Assert.AreEqual("content", filesystem.File.ReadAllText("./pack.zip"));
    }
}