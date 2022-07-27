using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IChain : IOperate, IObservable<WatchingEventInfo>, IList<IOperate>
{
    /// <summary>
    /// name of this Chain, will initialization on <see cref="Initialization(string,bool)"/> or <see cref="Initialization(Guid,string,bool)"/>
    /// </summary>
    string Name { get; set; }

    void Initialization(string name, bool isReadOnly = false);

    void Initialization(Guid guid, string name, bool isReadOnly = false);
}