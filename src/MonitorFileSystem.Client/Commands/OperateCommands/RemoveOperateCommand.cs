using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class RemoveOperateCommand : Command
{
    public RemoveOperateCommand()
        : base("remove", CommandTexts.Operate_Remove_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Operate_Remove_Guid_ArgumentDescription
        };

        AddArgument(guid);
        this.SetHandler<string>(Remove, guid);
    }

    private static async Task Remove(string guid)
    {
        var request = new GuidRequest
        {
            Guid = guid
        };

        await GrpcUnits.ActionManagementClient.RemoveOperateAsync(request);
    }
}