using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddUnpackOperateCommand : Command
{
    public AddUnpackOperateCommand() : base("unpack", CommandTexts.Operate_Add_Unpack_CommandDescription)
    {
        var destination = new Argument<string?>
        {
            Name = "destination",
            Description = CommandTexts.Operate_Add_Unpack_Destination_ArgumentDescription
        };
        destination.SetDefaultValue(null);

        var description = new Argument<string?>
        {
            Name = "description",
            Description = CommandTexts.Operate_Add_Unpack_Description_ArgumentDescription
        };
        description.SetDefaultValue(null);

        AddArgument(destination);
        AddArgument(description);
        this.SetHandler<string?, string?>(Create, destination, description);
    }

    private static async Task Create(string? destination, string? description)
    {
        var request = new UnpackOperateRequest();
        if (destination is not null)
        {
            request.Destination = destination;
        }

        if (description is not null)
        {
            request.Description = destination;
        }

        var response = await GrpcUnits.ActionManagementClient.CreateUnpackOperateAsync(request);

        Console.Write(response.Guid);
    }
}