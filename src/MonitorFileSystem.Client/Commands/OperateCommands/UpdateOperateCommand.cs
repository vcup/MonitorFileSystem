using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class UpdateOperateCommand : Command
{
    public UpdateOperateCommand() : base("update")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
        };

        var description = new Argument<string?>
        {
            Name = "description"
        };
        description.SetDefaultValue(null);
        
        AddArgument(guid);
        AddArgument(description);
        this.SetHandler<string, string?>(UpdateOperate, guid, description);
        
        AddCommand(new UpdateMoveOperateCommand());
        AddCommand(new UpdateUnpackOperateCommand());
    }

    private static async Task UpdateOperate(string guid, string? description)
    {
        var request = new OperateRequest
        {
            Guid = guid
        };
        if (description is not null)
        {
            request.Description = description;
        }

        await GrpcUnits.ActionManagementClient.UpdateOperateAsync(request);
    }
}