using System.CommandLine;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

public class OperateCommand : Command
{
    public OperateCommand()
        : base("operate", "default list operates")
    {
    }
}