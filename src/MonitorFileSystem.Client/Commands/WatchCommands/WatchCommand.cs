using System.CommandLine;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class WatchCommand : Command
{
    public WatchCommand() : base("watch", CommandTexts.Watch_Command_Description)
    {
    }
}