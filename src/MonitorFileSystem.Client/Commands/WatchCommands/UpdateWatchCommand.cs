using System.CommandLine;
using MonitorFileSystem.Client.Grpc;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class UpdateWatchCommand : Command
{
    public UpdateWatchCommand()
        : base("update", "update a watcher for target grpc service")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
        };

        var name = new Argument<string?>
        {
            Name = "name"
        };
        name.SetDefaultValue(null);

        var path = new Argument<string?>
        {
            Name = "path"
        };
        path.SetDefaultValue(null);

        var filter = new Argument<string?>
        {
            Name = "filter"
        };
        filter.SetDefaultValue(null);
        
        var @event = new Argument<WatchingEvent?>
        {
            Name = "event"
        };
        @event.SetDefaultValue(null);

        AddArgument(guid);
        AddArgument(name);
        AddArgument(path);
        AddArgument(filter);
        AddArgument(@event);
        
        this.SetHandler<string, string?, string?, string?, WatchingEvent?>(
            UpdateWatcher, guid, name, path, filter, @event);
    }

    private static async Task UpdateWatcher(string guid, string? name, string? path, string? filter, WatchingEvent? @event)
    {
        var request = new UpdateWatcherRequest
        {
            Guid = guid
        };

        if (name is not null)
        {
            request.Name = name;
        }

        if (path is not null)
        {
            request.Path = path;
        }

        if (filter is not null)
        {
            request.Filter = filter;
        }

        if (@event.HasValue)
        {
            request.EventFlags = (int)@event.Value;
        }

        await GrpcUnits.MonitorManagementClient.UpdateWatcherAsync(request);
    }
}