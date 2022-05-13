using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class RemoveWatchCommand : Command
{
    public RemoveWatchCommand()
        : base("remove", "remove watcher from target grpc service")
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = "guid of Watcher"
        };
        AddArgument(guid);
        
        this.SetHandler<string>(RemoveWatcher, guid);
    }

    private static async Task RemoveWatcher(string guid)
    {
        var request = new GuidRequest
        {
            Guid = guid
        };

        await GrpcUnits.MonitorManagementClient.RemoveWatcherAsync(request);
    }
}