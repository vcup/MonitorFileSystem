using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class UpdateOperateCommand : Command
{
    public UpdateOperateCommand()
        : base("update", CommandTexts.Operate_Update_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Operate_Update_Guid_ArgumentDescription
        };

        var description = new Argument<string?>
        {
            Name = "description",
            Description = CommandTexts.Operate_Update_Description_ArgumentDescription
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