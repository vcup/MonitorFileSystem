using System.CommandLine;
using MonitorFileSystem.Client.Commands.WatchCommands;

namespace MonitorFileSystem.Client.Commands;

public static class StartupCommands
{
    public static Command AddClientCommands(this Command command)
    {
        var watch = new WatchCommand();
        watch.AddCommand(new AddWatchCommand());
        
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