using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.LinkCommands;

internal class LinkCommand : Command
{
    public LinkCommand()
        : base("link", CommandTexts.Link_CommandDescription)
    {
        var monitor = new Argument<string>
        {
            Name = "monitor",
            Description = CommandTexts.Link_Monitor_ArgumentDescription
        };

        var action = new Argument<string>
        {
            Name = "action",
            Description = CommandTexts.Link_Action_ArgumentDescription
        };

        AddArgument(monitor);
        AddArgument(action);
        this.SetHandler<string, string>(Attach, monitor, action);
    }

    private static async Task Attach(string monitor, string action)
    {
        var request = new MonitorAndActionRequest
        {
            Monitor = new GuidRequest { Guid = monitor },
            Action = new GuidRequest { Guid = action }
        };

        await GrpcUnits.MasterManagementClient.AttachActionToMonitorAsync(request);
    }
}