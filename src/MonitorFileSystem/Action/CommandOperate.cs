using System.Diagnostics;
using System.IO.Abstractions;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public class CommandOperate : OperateBase, ICommandOperate
{
    private readonly ProcessStartInfo _startInfo;
    
    public CommandOperate(IFileSystem fileSystem, ILogger<CommandOperate> logger)
        : base(fileSystem, logger)
    {
        Arguments = null!;
        CommandLineTemplate = string.Empty;
#if Windows
        _startInfo = new ProcessStartInfo("cmd.exe");
#elif Linux
        _startInfo = new ProcessStartInfo("/usr/bin/sh");
#endif
        _startInfo.UseShellExecute = false;
        _startInfo.RedirectStandardInput = _startInfo.RedirectStandardOutput = _startInfo.RedirectStandardError = true;
    }

    public string CommandLineTemplate { get; set; }
    public IList<CommandOperateArgument> Arguments { get; private set; }

    public void Initialization(Guid guid, string command)
    {
        CheckIsNotInitialized();
        CommandLineTemplate = command;
    }

    public void Initialization(Guid guid, string command, params CommandOperateArgument[] arguments)
    {
        Initialization(guid, command);
        Arguments = arguments.ToList();
    }

    public void Initialization(string command)
    {
        Initialization(Guid.NewGuid(), command);
    }

    public void Initialization(string command, params CommandOperateArgument[] arguments)
    {
        Initialization(Guid.NewGuid(), command, arguments);
    }

    public override void Process(WatchingEventInfo info)
    {
        //  will throw exception on unit test, because WorkingDirectory do not using IFileSystem
        _startInfo.WorkingDirectory = FileSystem.Path.GetDirectoryName(info.Path);
        _startInfo.Arguments = string.Format(CommandLineTemplate, GetActualCommandLine());

        using var process = new Process();
        process.StartInfo = _startInfo;
        
        process.Start();
        
        object?[] GetActualCommandLine()
        {
            var result = new List<object?>();
            foreach (var argument in Arguments)
            {
                switch (argument)
                {
                    case CommandOperateArgument.Name:
                        result.Add(GetNameOfPath(info.Path));
                        break;
                    case CommandOperateArgument.Path:
                        result.Add(info.Path);
                        break;
                    case CommandOperateArgument.OldName:
                        result.Add(GetNameOfPath(info.OldPath));
                        break;
                    case CommandOperateArgument.OldPath:
                        result.Add(info.OldPath);
                        break;
                }
            }
            
            return result.ToArray();
        }
    }
    
    private static string? GetNameOfPath(string? path)
    {
        return path?.Replace('\\', '/')
            .Split('/')
            .Last(str => !string.IsNullOrEmpty(str));
    }
}