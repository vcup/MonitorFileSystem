using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddMoveOperateCommand : Command
{
    public AddMoveOperateCommand() : base("move", CommandTexts.Operate_Add_Move_CommandDescription)
    {
        var destination = new Argument<string>
        {
            Name = "destination",
            Description = CommandTexts.Operate_Add_Move_Destination_ArgumentDesciption
        };

        var description = new Argument<string?>
        {
            Name = "description",
            Description = CommandTexts.Operate_Add_Move_Description_ArgumentDescription
        };
        description.SetDefaultValue(null);

        AddArgument(destination);
        AddArgument(description);
        this.SetHandler<string, string?>(Create, destination, description);
    }

    private static async Task Create(string destination, string? description)
    {
        var request = new MoveOperateRequest
        {
            Destination = destination
        };
        if (description is not null)
        {
            request.Description = description;
        }

        var response = await GrpcUnits.ActionManagementClient.CreateMoveOperateAsync(request);

        Console.WriteLine(response.Guid);
    }
}