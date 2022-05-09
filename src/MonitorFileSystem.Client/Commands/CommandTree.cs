using System.CommandLine;

namespace MonitorFileSystem.Client.Commands;

public sealed class CommandTree
{
    private readonly RootCommand _command;
    private readonly string[] _args;

    public CommandTree(RootCommand command, string[] args)
    {
        _command = command;
        _args = args;
    }

    public async Task<int> InvokeAsync()
    {
        var result = _command.Parse(_args);
        return await _command.InvokeAsync(_args);
    }
}