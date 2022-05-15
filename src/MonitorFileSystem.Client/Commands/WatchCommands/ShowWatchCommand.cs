using System.CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class ShowWatchCommand : Command
{
    public ShowWatchCommand()
        : base("show", CommandTexts.Watch_Show_CommandDescription)
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