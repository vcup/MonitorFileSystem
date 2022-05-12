using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class UpdateUnpackOperateCommand : Command
{
    public UpdateUnpackOperateCommand() : base("unpack")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
        };

        var destination = new Argument<string?>
        {
            Name = "destination"
        };
        destination.SetDefaultValue(null);

        var description = new Argument<string?>
        {
            Name = "description"
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