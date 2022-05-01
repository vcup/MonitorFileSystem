using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public class MoveOperate : OperateBase
{
    // see Initialization()
    private string _destination = null!;

    internal MoveOperate(ILogger<MoveOperate> logger)
        : this(logger, new FileSystem())
    {
    }

    internal MoveOperate(ILogger<MoveOperate> logger, IFileSystem fileSystem)
        : base(logger, fileSystem)
    {
    }

    public override void Initialization(params object[] parameters)
    {
        _destination = (string)parameters[0];
        base.Initialization(parameters);
    }

    private bool? IsFile(string path)
    {
        if (FileSystem.File.Exists(path))
        {
            return true;
        }

        if (FileSystem.Directory.Exists(path))
        {
            return false;
        }

        return null;
    }

    public override void Process(WatchingEventInfo info)
    {
        CheckIsInitialized();
        var destIsFile = IsFile(info.Path);

        if (FileSystem.File.Exists(info.Path))
        {
            FileSystem.File.Move(info.Path, _destination, true);
            // if destIsFile is null, dest path also is a File. because a file is Moved to dest path
            info.Path = destIsFile ?? true ? _destination : Path.Join(_destination, Path.GetFileName(info.Path));
            
            Logger.LogTrace("MoveOperate Process on File branch, {path} -> {dest}", info.Path, _destination);
        }
        else
        {
            FileSystem.Directory.Move(info.Path, _destination);
            // if destIsFile is null, dest path also is a directory. because a directory is Moved to dest path
            info.Path = !destIsFile ?? true ? _destination : Path.Join(_destination, Path.GetFileName(info.Path));
            
            Logger.LogInformation("MoveOperate Process on Directory branch, {path} -> {dest}", info.Path, _destination);
        }
    }
}
