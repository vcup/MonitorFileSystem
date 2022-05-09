using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.WatchCommands;

internal class AddWatchCommand : Command
{
    private readonly MonitorManagement.MonitorManagementClient _client;
    
    public AddWatchCommand(MonitorManagement.MonitorManagementClient client)
        : base("add", "Create a watcher")
    {
        _client = client;
        
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

        AddArgument(path);
        AddArgument(filter);
        AddArgument(name);
        
        this.SetHandler<string, string, string>
            (AddWatcher, name, path, filter);
    }

    private void AddWatcher(string name, string path, string filter)
    {
        var response = _client.CreateWatcher(new WatcherRequest
        {
            Name = name,
            Path = path,
            Filter = filter,
            // EventValue = @event
        });
        
        Console.WriteLine(response.Guid);
    }
}