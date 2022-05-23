using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.LinkCommands;

internal class UnlinkCommand : Command
{
    public UnlinkCommand()
        : base("unlink", CommandTexts.Unlink_CommandDescription)
    {
        var monitor = new Argument<string>
        {
            Name = "monitor",
            Description = CommandTexts.Unlink_Monitor_ArgumentDescription
        };

        var action = new Argument<string>
        {
            Name = "action",
            Description = CommandTexts.Unlink_Action_ArgumentDescription
        };

        AddArgument(monitor);
        AddArgument(action);

        this.SetHandler<string, string>(Detach, monitor, action);
    }

    private static async Task Detach(string monitor, string action)
    {
        var request = new MonitorAndActionRequest
        {
            Monitor = new GuidRequest { Guid = monitor },
            Action = new GuidRequest { Guid = action }
        };

        await GrpcUnits.MasterManagementClient.DetachActionToMonitorAsync(request);
    }
}