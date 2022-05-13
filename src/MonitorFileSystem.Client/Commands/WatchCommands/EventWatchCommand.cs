using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class EventWatchCommand : Command
{
    public EventWatchCommand()
        : base("event", "set a watcher watching filesystem event for target grpc service")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
        };

        var @event = new Argument<WatchingEvent>
        {
            Name = "event"
        };

        var add = new Command("add", "add watching event for watcher");
        add.AddArgument(guid);
        add.AddArgument(@event);
        add.SetHandler<string, WatchingEvent>(Add, guid, @event);
        
        var remove = new Command("remove", "remove watching event for watcher");
        remove.AddArgument(guid);
        remove.AddArgument(@event);
        remove.SetHandler<string, WatchingEvent>(Remove, guid, @event);
        
        var show = new Command("show", "show a watcher watching filesystem events");
        show.AddArgument(guid);
        show.SetHandler<string>(Show, guid);
        
        var set = new Command("set", "set watcher watching event");
        set.AddArgument(guid);
        set.AddArgument(@event);
        set.SetHandler<string, WatchingEvent>(Set, guid, @event);
        
        AddCommand(add);
        AddCommand(remove);
        AddCommand(show);
        AddCommand(set);
        
        AddArgument(guid);
        this.SetHandler<string>(Show, guid);
    }

    private static async Task Add(string guid, WatchingEvent @event)
    {
        var request = new UpdateWatcherEventRequest
        {
            Guid = guid,
            EventFlags = (int)@event
        };

        await GrpcUnits.MonitorManagementClient.AddWatcherMonitorEventAsync(request);
    }
    
    private static async Task Remove(string guid, WatchingEvent @event)
    {
        var request = new UpdateWatcherEventRequest
        {
            Guid = guid,
            EventFlags = (int)@event
        };

        await GrpcUnits.MonitorManagementClient.RemoveWatcherMonitorEventAsync(request);
    }
    
    private static async Task Show(string guid)
    {
        var request = new GuidRequest
        {
            Guid = guid,
        };

        var response = await GrpcUnits.MonitorManagementClient.FindWatcherAsync(request);
        
        Console.WriteLine((WatchingEvent)response.Event);
    }
    
    private static async Task Set(string guid, WatchingEvent @event)
    {
        var request = new UpdateWatcherEventRequest
        {
            Guid = guid,
            EventFlags = (int)@event
        };

        await GrpcUnits.MonitorManagementClient.SetWatcherMonitorEventAsync(request);
    }
}