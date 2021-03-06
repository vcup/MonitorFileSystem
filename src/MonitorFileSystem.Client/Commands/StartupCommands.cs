using System.CommandLine;
using MonitorFileSystem.Client.Commands.LinkCommands;
using MonitorFileSystem.Client.Commands.OperateCommands;
using MonitorFileSystem.Client.Commands.WatchCommands;

namespace MonitorFileSystem.Client.Commands;

public static class StartupCommands
{
    public static Command AddWatchCommands(this Command command)
    {
        var watch = new WatchCommand();

        watch.AddCommand(new AddWatchCommand());
        watch.AddCommand(new RemoveWatchCommand());
        watch.AddCommand(new ShowWatchCommand());
        watch.AddCommand(new UpdateWatchCommand());
        watch.AddCommand(new EventWatchCommand());
        watch.SetHandler(ShowWatchCommand.ShowWatchers);

        command.AddCommand(watch);
        return command;
    }

    public static Command AddOperateCommands(this Command command)
    {
        var operate = new OperateCommand();
        operate.AddCommand(new AddOperateCommand());
        operate.AddCommand(new RemoveOperateCommand());
        operate.AddCommand(new ShowOperateCommand());
        operate.AddCommand(new UpdateOperateCommand());

        operate.SetHandler(ShowOperateCommand.ShowOperate);
        command.AddCommand(operate);
        return command;
    }

    public static Command AddLinkCommands(this Command command)
    {
        command.AddCommand(new LinkCommand());
        command.AddCommand(new ShowRelationCommand());
        command.AddCommand(new UnlinkCommand());

        return command;
    }

    public static Command AddGlobalOptions(this Command command)
    {
        foreach (var property in typeof(GlobalOptions).GetProperties())
        {
            var value = property.GetValue(typeof(GlobalOptions));
            if (value is Option option)
            {
                command.Add(option);
            }
        }

        return command;
    }
}