using NUnit.Framework;
using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

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

        var watcher = new Monitor.Watcher("Tester", "./", "*", @event , fileSystem);

        Assert.AreEqual(watcher.Monitoring, mustBe);
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
}