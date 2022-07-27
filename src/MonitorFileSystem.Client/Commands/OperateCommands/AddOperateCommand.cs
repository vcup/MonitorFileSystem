using System.CommandLine;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddOperateCommand : Command
{
    public AddOperateCommand()
        : base("add", CommandTexts.Operate_Add_CommandDescription)
    {
        AddCommand(new AddMoveOperateCommand());
        AddCommand(new AddUnpackOperateCommand());
        AddCommand(new AddCommandOperateCommand());
    }
}