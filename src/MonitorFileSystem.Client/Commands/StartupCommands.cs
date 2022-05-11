using System.CommandLine;
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
        var showCommand = new ShowWatchCommand();
        watch.AddCommand(showCommand);
        watch.SetHandler(showCommand.ShowWatchers);
        watch.AddCommand(new UpdateWatchCommand());
        watch.AddCommand(new EventWatchCommand());
        
        command.AddCommand(watch);
        return command;
    }

    public static Command AddOperateCommands(this Command command)
    {
        var operate = new OperateCommand();

        command.AddCommand(operate);
        return command;
    }

    public static Command AddGlobalOptions(this Command command)
    {
        var globalOptions = new GlobalOptions();
        foreach (var property in typeof(GlobalOptions).GetProperties())
        {
            var value = property.GetValue(globalOptions);
            if (value is Option option)
            {
                command.Add(option);
            }
        }

        return command;
    }
}