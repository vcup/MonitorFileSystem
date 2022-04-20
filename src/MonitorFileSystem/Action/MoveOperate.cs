using MonitorFileSystem.Monitor;
using System.IO;

namespace MonitorFileSystem.Action;

internal class MoveOperate : OperateBase
{
    private readonly string _destination;

    public MoveOperate(string destination, ILogger<MoveOperate> logger) : base(logger)
    {
        _destination = destination;
    }

    private bool? isFile(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else if (Directory.Exists(path))
        {
            return false;
        }

        return null;
    }

    public override void Process(WatchingEventInfo info)
    {
        var destIsFile = isFile(info.Path);

        if (File.Exists(info.Path))
        {
            File.Move(info.Path, _destination, true);
            // if destIsFile is null, dest path also is a File. becuse a file is Moved to dest path
            info.Path = destIsFile ?? true ? _destination : Path.Join(_destination, Path.GetFileName(info.Path));
        }
        else
        {
            Directory.Move(info.Path, _destination);
            // if destIsFile is null, dest path also is a directory. becuse a directory is Moved to dest path
            info.Path = !destIsFile ?? true ? _destination : Path.Join(_destination, Path.GetFileName(info.Path));
        }

    }

    public override Task ProcessAsync(WatchingEventInfo info)
    {
        return Task.Run(() => Process(info));
    }
}
