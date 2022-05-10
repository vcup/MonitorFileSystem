using System.CommandLine;
using MonitorFileSystem.Client.Commands.WatchCommands;

namespace MonitorFileSystem.Client.Commands;

public static class StartupCommands
{
    public static Command AddWatchCommands(this Command command)
    {
        var watch = new WatchCommand();
        watch.AddCommand(new AddWatchCommand());
        var showCommand = new ShowWatchCommand();
        watch.AddCommand(showCommand);
        watch.SetHandler(showCommand.ShowWatchers);
        
        command.AddCommand(watch);
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