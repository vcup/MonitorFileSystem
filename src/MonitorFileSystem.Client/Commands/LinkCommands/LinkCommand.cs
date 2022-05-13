using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.LinkCommands;

internal class LinkCommand : Command
{
    public LinkCommand() : base("link")
    {
        var monitor = new Argument<string>
        {
            Name = "monitor",
            Description = "Guid of watcher or group"
        };

        var action = new Argument<string>
        {
            Name = "action",
            Description = "Guid of operate or chain"
        };

        AddArgument(monitor);
        AddArgument(action);
        this.SetHandler<string, string>(Attach, monitor, action);
    }

    private static async Task Attach(string monitor, string action)
    {
        var request = new MonitorAndActionRequest
        {
            Monitor = new GuidRequest{Guid = monitor},
            Action = new GuidRequest{Guid = action}
        };

        await GrpcUnits.MasterManagementClient.AttachActionToMonitorAsync(request);
    }
}