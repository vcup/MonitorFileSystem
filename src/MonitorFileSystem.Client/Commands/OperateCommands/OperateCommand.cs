using System.CommandLine;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class OperateCommand : Command
{
    public OperateCommand()
        : base("operate", "default list operates")
    {
    }
}