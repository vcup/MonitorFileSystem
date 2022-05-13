using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddMoveOperateCommand : Command
{
    public AddMoveOperateCommand() : base("move")
    {
        var destination = new Argument<string>
        {
            Name = "destination"
        };

        var description = new Argument<string>
        {
            Name = "description"
        };
        description.SetDefaultValue(string.Empty);
        
        AddArgument(destination);
        AddArgument(description);
        this.SetHandler<string, string>(Create, destination, description);
    }

    private static async Task Create(string destination, string description)
    {
        var request = new MoveOperateRequest
        {
            Destination = destination,
            Description = description
        };

        var response = await GrpcUnits.ActionManagementClient.CreateMoveOperateAsync(request);

        Console.WriteLine(response.Guid);
    }
}