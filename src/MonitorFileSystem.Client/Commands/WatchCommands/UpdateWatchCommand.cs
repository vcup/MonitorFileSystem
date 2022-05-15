using System.CommandLine;
using MonitorFileSystem.Client.Resources;
using MonitorFileSystem.Grpc.ProtocolBuffers;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class UpdateWatchCommand : Command
{
    public UpdateWatchCommand()
        : base("update", CommandTexts.Watch_Update_CommandDescription)
    {
        var guid = new Argument<string>
        {
            Name = "guid",
            Description = CommandTexts.Watch_Update_Guid_ArgumentDescription
        };

        var name = new Argument<string?>
        {
            Name = "name",
            Description = CommandTexts.Watch_Update_Name_ArgumentDescription
        };
        name.SetDefaultValue(null);

        var path = new Argument<string?>
        {
            Name = "path",
            Description = CommandTexts.Watch_Update_Path_ArgumentDescription
        };
        path.SetDefaultValue(null);

        var filter = new Argument<string?>
        {
            Name = "filter",
            Description = CommandTexts.Watch_Update_Filter_ArgumentDescription
        };
        filter.SetDefaultValue(null);
        
        var @event = new Argument<WatchingEvent?>
        {
            Name = "event",
            Description = CommandTexts.Watch_Update_Event_ArgumentDescription
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