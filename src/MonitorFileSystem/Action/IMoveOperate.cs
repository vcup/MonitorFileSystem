using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public interface IMoveOperate : IOperate
{
    void Initialization(string destination);
}