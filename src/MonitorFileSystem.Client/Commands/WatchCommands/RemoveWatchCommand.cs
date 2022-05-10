using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
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

    private void RemoveWatcher(string guid)
    {
        var request = new GuidRequest
        {
            Guid = guid
        };

        GrpcUnits.MonitorManagementClient.RemoveWatcher(request);
    }
}