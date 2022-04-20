namespace MonitorFileSystem.Action;

internal interface IChain : IOperate, IEnumerable<IOperate>
{
    string Name { get; }
    string Description { get; }
}
