namespace MonitorFileSystem.Action;

public interface IUnpackOperate : IOperate
{
    string? Destination { get; set; }
}