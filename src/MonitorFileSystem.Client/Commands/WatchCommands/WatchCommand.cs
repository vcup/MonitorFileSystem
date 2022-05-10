using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class WatchCommand : Command
{
    public WatchCommand(MonitorManagement.MonitorManagementClient client) : base("watch", "default list watchers")
    {
        AddCommand(new AddWatchCommand(client));
    }
}