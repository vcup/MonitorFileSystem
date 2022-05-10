using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

public class ShowWatchCommand : Command
{
    public ShowWatchCommand()
        : base("show", "show all watcher of target grpc service")
    {
        this.SetHandler(ShowWatchers);
    }

    internal async Task ShowWatchers()
    {
        var request = new Empty();

        var response = GrpcUnits.MonitorManagementClient.GetWatchers(request);
        await foreach (var watchers in response.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(watchers.Guid);
        }
    }
}