using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class ShowOperateCommand : Command
{
    public ShowOperateCommand() : base("show")
    {
        this.SetHandler(ShowOperate);
    }

    internal static async Task ShowOperate()
    {
        var request = new Empty();

        var responses = GrpcUnits.ActionManagementClient.GetOperates(request);

        await foreach (var response in responses.ResponseStream.ReadAllAsync())
        {
            switch (response.OperateCase)
            {
                case OperateResponse.OperateOneofCase.None:
                    break;
                case OperateResponse.OperateOneofCase.Move:
                    Console.WriteLine(response.Move.Guid);
                    break;
                case OperateResponse.OperateOneofCase.Unpack:
                    Console.WriteLine(response.Unpack.Guid);
                    break;
            }
        }
    }
}