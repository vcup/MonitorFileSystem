using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

internal interface IChain : IOperate, IObservable<WatchingEventInfo>, IEnumerable<IOperate>
{
    string Name { get; }
    string? Description { get; }
}
