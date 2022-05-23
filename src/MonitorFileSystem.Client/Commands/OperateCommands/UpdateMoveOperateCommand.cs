using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class UpdateMoveOperateCommand : Command
{
    public UpdateMoveOperateCommand()
        : base("move", CommandTexts.Operate_Update_Move_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Operate_Update_Move_Guid_ArgumentDescription
        };

        var destination = new Argument<string?>
        {
            Name = "destination",
            Description = CommandTexts.Operate_Update_Move_Destination_ArgumentDescription
        };
        destination.SetDefaultValue(null);

        var description = new Argument<string?>
        {
            Name = "description",
            Description = CommandTexts.Operate_Update_Move_Description_ArgumentDescription
        };
        description.SetDefaultValue(null);

        AddArgument(guid);
        AddArgument(destination);
        AddArgument(description);
        this.SetHandler<string, string?, string?>(UpdateMove, guid, destination, description);
    }

    private static async Task UpdateMove(string guid, string? destination, string? description)
    {
        if (destination is null && description is null)
        {
            return;
        }

        var request = new UpdateMoveOperateRequest
        {
            Guid = guid
        };

        if (destination is not null)
        {
            request.Destination = destination;
        }

        if (description is not null)
        {
            request.Description = description;
        }

        await GrpcUnits.ActionManagementClient.UpdateMoveOperateAsync(request);
    }
}