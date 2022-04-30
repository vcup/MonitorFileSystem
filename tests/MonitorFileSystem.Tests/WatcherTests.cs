using NUnit.Framework;
using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO;

namespace MonitorFileSystem.Tests;

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
        public TestFileSystem(IFileSystemWatcherFactory fileSystemWatcherFactory)
        {
            this.FileSystemWatcher = fileSystemWatcherFactory;
        }

        public override IFileSystemWatcherFactory FileSystemWatcher { get; }
    }
    
    private class TestFileSystemWatcher : IFileSystemWatcherFactory
    {
        private IFileSystemWatcher? _lastCreatedFileSystemWatcher;

        public TestFileSystemWatcher()
        {
        }

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
            return _lastCreatedFileSystemWatcher = _lastCreatedFileSystemWatcher ?? new FileSystemWatcherWrapper(path, filter);
        }
    }
}