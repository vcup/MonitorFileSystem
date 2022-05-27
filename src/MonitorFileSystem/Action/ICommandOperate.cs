namespace MonitorFileSystem.Action;

public interface ICommandOperate : IOperate
{
    string CommandLineTemplate { get; set; }
    List<CommandOperateArgument> Arguments { get; }

    void Initialization(Guid guid, string command);

    void Initialization(Guid guid, string command, params CommandOperateArgument[] arguments);

    void Initialization(string command);

    void Initialization(string command, params CommandOperateArgument[] arguments);
}