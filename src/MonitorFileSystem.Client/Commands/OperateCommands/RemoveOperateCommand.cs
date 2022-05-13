using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class RemoveOperateCommand : Command
{
    public RemoveOperateCommand() : base("remove")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
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