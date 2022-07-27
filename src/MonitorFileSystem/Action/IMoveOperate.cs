namespace MonitorFileSystem.Action;

public interface IMoveOperate : IOperate
{
    string Destination { get; set; }

    void Initialization(string destination);

    void Initialization(Guid guid, string destination);
}