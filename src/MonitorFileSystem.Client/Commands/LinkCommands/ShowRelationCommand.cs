using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.LinkCommands;

internal class ShowRelationCommand : Command
{
    public ShowRelationCommand() : base("show")
    {
        var guid = new Argument<string?>
        {
            Name = "guid",
            Description = "guid of Monitor or Action"
        };
        guid.SetDefaultValue(null);
        
        AddArgument(guid);
        this.SetHandler<string?>(ShowRelation, guid);
    }

    private static async Task ShowRelation(string? guid)
    {
        if (guid is null)
        {
            var request = new Empty();
            var responses = GrpcUnits.MasterManagementClient.GetRelations(request);

            await foreach (var response in responses.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("{0} -> {1}", response.Monitor.Guid, response.Action.Guid);
            }
        }
        else
        {
            var request = new GuidRequest
            {
                Guid = guid
            };
            var responses = GrpcUnits.MasterManagementClient.GetRelationOfEither(request);

            await foreach (var response in responses.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("{0} -> {1}", guid, response.Guid);
            }
        }
    }
}