using NUnit.Framework;
using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Common;

namespace MonitorFileSystem.Tests;

public class WatcherTests : InitializableBaseTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Provider = new HostBuilder()
            .ConfigureServices(service =>
            {
                service.AddScoped<IInitializable, Watcher>()
                    .AddScoped<IWatcher, Watcher>()
                    .AddScoped<IFileSystem, TestFileSystem>()
                    .AddScoped<IFileSystemWatcherFactory, TestFileSystemWatcher>();
            })
            .Build()
            .Services;
    }

    [SetUp]
    public void Setup()
    {
    }

    [TestCase(WatchingEvent.None, false)]
    [TestCase(WatchingEvent.Created, false)]
    [TestCase(WatchingEvent.Created | WatchingEvent.FileName, true)]
    [TestCase(WatchingEvent.FileName, true)]
    public void Monitoring_WatchingEventSetter_FalseOnEventIsNoneTest(WatchingEvent @event, bool mustBe)
    {
        var scoped = Provider.CreateScope();

        var watcher = scoped.ServiceProvider.GetRequiredService<IWatcher>();
        watcher.Initialization(@event);

        Assert.AreEqual(mustBe, watcher.Monitoring);
    }

    [TestCase(WatchingEvent.FileName     , NotifyFilters.FileName)]
    [TestCase(WatchingEvent.DirectoryName, NotifyFilters.DirectoryName)]
    [TestCase(WatchingEvent.Attributes   , NotifyFilters.Attributes)]
    [TestCase(WatchingEvent.Size         , NotifyFilters.Size)]
    [TestCase(WatchingEvent.LastWrite    , NotifyFilters.LastWrite)]
    [TestCase(WatchingEvent.LastAccess   , NotifyFilters.LastAccess)]
    [TestCase(WatchingEvent.CreationTime , NotifyFilters.CreationTime)]
    [TestCase(WatchingEvent.Security, NotifyFilters.Security)]
    public void NotifyFilter_WatchingEventSetter_MustSameOfWatchingEvent(WatchingEvent @event, NotifyFilters notifyFilters)
    {
        var scoped = Provider.CreateScope();

        var factory = scoped.ServiceProvider.GetRequiredService<IFileSystemWatcherFactory>() as TestFileSystemWatcher;
        Assert.IsNotNull(factory);
        var watcher = scoped.ServiceProvider.GetRequiredService<IWatcher>();
        watcher.Initialization(@event);

        Assert.IsNotNull(factory!.LastCreatedFileSystemWatcher);
        Assert.AreEqual(notifyFilters, factory.LastCreatedFileSystemWatcher!.NotifyFilter);
    }

    private class TestFileSystem : MockFileSystem
    {
        public TestFileSystem(IFileSystemWatcherFactory fileSystemWatcherFactory)
        {
            FileSystemWatcher = fileSystemWatcherFactory;
        }

        public override IFileSystemWatcherFactory FileSystemWatcher { get; }
    }

    private class TestFileSystemWatcher : IFileSystemWatcherFactory
    {
        private IFileSystemWatcher? _lastCreatedFileSystemWatcher;

        public IFileSystemWatcher? LastCreatedFileSystemWatcher => _lastCreatedFileSystemWatcher;

        public IFileSystemWatcher CreateNew()
        {
            return _lastCreatedFileSystemWatcher = _lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper();
        }

        public IFileSystemWatcher CreateNew(string path)
        {
            return _lastCreatedFileSystemWatcher = _lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper(path);
        }

        public IFileSystemWatcher CreateNew(string path, string filter)
        {
            return _lastCreatedFileSystemWatcher =
                _lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper(path, filter);
        }
    }
}