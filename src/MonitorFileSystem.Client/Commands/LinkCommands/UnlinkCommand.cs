using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.LinkCommands;

internal class UnlinkCommand : Command
{
    public UnlinkCommand() : base("unlink")
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
    }

    private static async Task Detach(string monitor, string action)
    {
        var request = new MonitorAndActionRequest
        {
            Monitor = new GuidRequest{Guid = monitor},
            Action = new GuidRequest{Guid = action}
        };

        await GrpcUnits.MasterManagementClient.DetachActionToMonitorAsync(request);
    }
}