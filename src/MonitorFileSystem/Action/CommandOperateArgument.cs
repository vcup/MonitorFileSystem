namespace MonitorFileSystem.Action;

public enum CommandOperateArgument
{
    Name    = 0b0001,
    Path    = 0b0010,
    OldName = 0b0100,
    OldPath = 0b1000
}