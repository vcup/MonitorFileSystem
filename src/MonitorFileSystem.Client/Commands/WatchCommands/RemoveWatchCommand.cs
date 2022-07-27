using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class RemoveWatchCommand : Command
{
    public RemoveWatchCommand()
        : base("remove", CommandTexts.Watch_Remove_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Watch_Remove_Guid_ArgumentDescription
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