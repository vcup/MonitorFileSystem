using System.CommandLine;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class WatchCommand : Command
{
    public WatchCommand() : base("watch", "default list watchers")
    {
    }
}