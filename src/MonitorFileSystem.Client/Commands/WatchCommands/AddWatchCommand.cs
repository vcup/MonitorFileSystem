using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class AddWatchCommand : Command
{
    public AddWatchCommand()
        : base("add", "Create a watcher")
    {
        var name = new Argument<string>
        {
            Name = "name",
            Description = "name of watcher"
        };
        name.SetDefaultValue(string.Empty);

        var path = new Argument<string>
        {
            Name = "path",
            Description = "watching path",
        };

        var filter = new Argument<string>
        {
            Name = "filter",
            Description = "regular expression",
        };
        filter.SetDefaultValue("*");

        var @event = new Argument<Event>
        {
            Name = "event",
            Description = "watching filesystem events"
        };
        @event.SetDefaultValue(Event.None);

        AddArgument(path);
        AddArgument(filter);
        AddArgument(@event);
        AddArgument(name);
        
        this.SetHandler<string, string, string, Event>
            (AddWatcher, name, path, filter, @event);
    }

    private void AddWatcher(string name, string path, string filter, Event @event)
    {
        var request = new WatcherRequest
        {
            Name = name,
            Path = path,
            Filter = filter,
            Event = @event
        };
        var response = GrpcUnits.MonitorManagementClient.CreateWatcher(request);
        
        Console.WriteLine(response.Guid);
    }
}