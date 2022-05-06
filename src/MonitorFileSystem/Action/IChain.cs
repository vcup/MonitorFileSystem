using System.Diagnostics.CodeAnalysis;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IChain : IOperate, IObservable<WatchingEventInfo>, IList<IOperate>
{
    /// <summary>
    /// name of this Chain, will initialization on <see cref="Initialization(string,string,bool)"/>
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// description of this Chain, will initialization on <see cref="Initialization(string, string, bool)"/>
    /// </summary>
    string Description { get; }
    
    void Initialization(string name, string description = "", bool isReadOnly = false);
}
