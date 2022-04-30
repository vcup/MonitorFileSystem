using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

internal interface IChain : IOperate, IObservable<WatchingEventInfo>, IList<IOperate>
{
    string Name { get; }
    string? Description { get; }
}
