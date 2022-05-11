using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddUnpackOperateCommand : Command
{
    public AddUnpackOperateCommand() : base("unpack")
    {
        var destination = new Argument<string>
        {
            Name = "destination"
        };
        destination.SetDefaultValue(string.Empty);

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
        var request = new UnpackOperateRequest
        {
            Destination = destination,
            Description = description,
            IgnoreDirectory = true
        };

        var response = await GrpcUnits.ActionManagementClient.CreateUnpackOperateAsync(request);
        
        Console.Write(response.Guid);
    }
}