using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class AddWatchCommand : Command
{
    public AddWatchCommand()
        : base("add", "Create a watcher")
    {
        var name = new Argument<string>
        {
            Name = "name",
            Description = CommandTexts.Watch_Add_Name_Description
        };
        name.SetDefaultValue(string.Empty);

        var path = new Argument<string>
        {
            Name = "path",
            Description = CommandTexts.Watch_Add_Path_Description
        };

        var filter = new Argument<string>
        {
            Name = "filter",
            Description = CommandTexts.Watch_Add_Filter_Description
        };
        filter.SetDefaultValue("*");

        var @event = new Argument<WatchingEvent>
        {
            Name = "event",
            Description = CommandTexts.Watch_Add_Event_Description
        };
        @event.SetDefaultValue(WatchingEvent.None);

        AddArgument(path);
        AddArgument(filter);
        AddArgument(@event);
        AddArgument(name);
        
        this.SetHandler<string, string, string, WatchingEvent>
            (CreateWatcher, name, path, filter, @event);
    }

    private static async Task CreateWatcher(string name, string path, string filter, WatchingEvent @event)
    {
        var request = new WatcherRequest
        {
            Name = name,
            Path = path,
            Filter = filter,
            EventFlags = (int)@event
        };
        var response = await GrpcUnits.MonitorManagementClient.CreateWatcherAsync(request);
        
        Console.WriteLine(response.Guid);
    }
}