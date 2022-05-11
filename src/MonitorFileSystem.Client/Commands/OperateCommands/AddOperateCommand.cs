using System.CommandLine;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

public class AddOperateCommand : Command
{
    public AddOperateCommand()
        : base("add")
    {
        AddCommand(new AddMoveOperateCommand());
        AddCommand(new AddUnpackOperateCommand());
    }
}