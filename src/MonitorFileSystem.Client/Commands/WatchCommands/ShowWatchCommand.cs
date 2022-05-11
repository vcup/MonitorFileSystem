using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Client.Grpc;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class ShowWatchCommand : Command
{
    public ShowWatchCommand()
        : base("show", "show all watcher of target grpc service")
    {
        this.SetHandler(ShowWatchers);
    }

    internal static async Task ShowWatchers()
    {
        var request = new Empty();

        var response = GrpcUnits.MonitorManagementClient.GetWatchers(request);
        await foreach (var watchers in response.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(watchers.Guid);
        }
    }
}