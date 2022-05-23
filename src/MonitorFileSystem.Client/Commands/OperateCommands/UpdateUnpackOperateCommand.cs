using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class UpdateUnpackOperateCommand : Command
{
    public UpdateUnpackOperateCommand()
        : base("unpack", CommandTexts.Operate_Update_Unpack_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Operate_Update_Unpack_Guid_ArgumentDescription
        };

        var destination = new Argument<string?>
        {
            Name = "destination",
            Description = CommandTexts.Operate_Update_Unpack_Destination_ArgumentDescription
        };
        destination.SetDefaultValue(null);

        var description = new Argument<string?>
        {
            Name = "description",
            Description = CommandTexts.Operate_Update_Unpack_Description_ArgumentDescription
        };
        description.SetDefaultValue(null);

        AddArgument(guid);
        AddArgument(destination);
        AddArgument(description);
        this.SetHandler<string, string?, string?>(UpdateUnpack, guid, destination, description);
    }

    private static async Task UpdateUnpack(string guid, string? destination, string? description)
    {
        if (destination is null && description is null)
        {
            return;
        }

        var request = new UpdateUnpackOperateRequest
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

        await GrpcUnits.ActionManagementClient.UpdateUnpackOperateAsync(request);
    }
}