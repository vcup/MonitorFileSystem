using System.CommandLine;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class OperateCommand : Command
{
    public OperateCommand()
        : base("operate", CommandTexts.Operate_CommandDescription)
    {
    }
}