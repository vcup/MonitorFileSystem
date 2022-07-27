using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class ShowOperateCommand : Command
{
    public ShowOperateCommand()
        : base("show", CommandTexts.Operate_Show_CommandDescription)
    {
        this.SetHandler(ShowOperate);
    }

    public static async Task ShowOperate()
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