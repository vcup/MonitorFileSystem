namespace MonitorFileSystem.Action;

public interface IUnpackOperate : IOperate
{
    void Initialization(bool ignoreDirectory);
    void Initialization(string destination);
    void Initialization(string destination, bool ignoreDirectory);
}