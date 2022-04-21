using NUnit.Framework;
using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO;

namespace MonitorFileSystem.Test;

public class WatcherTests
{

    [SetUp]
    public void Setup()
    {
    }

    [TestCase(WatchingEvent.None, false)]
    [TestCase(WatchingEvent.Created, true)]
    public void Monitoring_WatchingEventSetter_FalseOnEventIsNoneTest(WatchingEvent @event, bool mustBe)
    {
        var fileSystem = new TestFileSystem(new FileSystemWatcherFactory());

        var watcher = new Watcher("Tester", "./", "*", @event , fileSystem);

        Assert.AreEqual(watcher.Monitoring, mustBe);
    }

    [TestCase(WatchingEvent.FileName     , NotifyFilters.FileName)]
    [TestCase(WatchingEvent.DirectoryName, NotifyFilters.DirectoryName)]
    [TestCase(WatchingEvent.Attributes   , NotifyFilters.Attributes)]
    [TestCase(WatchingEvent.Size         , NotifyFilters.Size)]
    [TestCase(WatchingEvent.LastWrite    , NotifyFilters.LastWrite)]
    [TestCase(WatchingEvent.LastAccess   , NotifyFilters.LastAccess)]
    [TestCase(WatchingEvent.CreationTime , NotifyFilters.CreationTime)]
    [TestCase(WatchingEvent.Security     , NotifyFilters.Security)]
    public void NotifyFilter_WatchingEventSetter_MustSameOfWatchingEvent(WatchingEvent @event, NotifyFilters filters)
    {
        var factory = new TestFileSystemWatcher();
        var fileSystem = new TestFileSystem(factory);

        var wathcer = new Watcher("Tester", "./", "*", @event, fileSystem);

        Assert.IsNotNull(factory.LastCreatedFileSystemWatcher);
        Assert.AreEqual(factory.LastCreatedFileSystemWatcher!.NotifyFilter, filters);
    }

    private class TestFileSystem : MockFileSystem
    {
        private readonly IFileSystemWatcherFactory fileSystemWatcherFactory;

        public TestFileSystem(IFileSystemWatcherFactory fileSystemWatcherFactory)
        {
            this.fileSystemWatcherFactory = fileSystemWatcherFactory;
        }

        public override IFileSystemWatcherFactory FileSystemWatcher => fileSystemWatcherFactory;
    }
    
    private class TestFileSystemWatcher : IFileSystemWatcherFactory
    {
        private IFileSystemWatcher? lastCreatedFileSystemWatcher;

        public TestFileSystemWatcher()
        {
        }

        public IFileSystemWatcher? LastCreatedFileSystemWatcher => lastCreatedFileSystemWatcher;

        public IFileSystemWatcher CreateNew()
        {
            return lastCreatedFileSystemWatcher = lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper();
        }

        public IFileSystemWatcher CreateNew(string path)
        {
            return lastCreatedFileSystemWatcher = lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper(path);
        }

        public IFileSystemWatcher CreateNew(string path, string filter)
        {
            return lastCreatedFileSystemWatcher = lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper(path, filter);
        }
    }
}