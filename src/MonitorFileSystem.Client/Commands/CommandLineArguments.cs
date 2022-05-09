namespace MonitorFileSystem.Client.Commands;

public class CommandLineArguments
{
    public CommandLineArguments(string[] args)
    {
        Arguments = args;
    }
    
    public string[] Arguments { get; }
}