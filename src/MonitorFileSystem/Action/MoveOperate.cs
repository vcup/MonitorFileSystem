using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public class MoveOperate : OperateBase, IMoveOperate
{
    public MoveOperate(IFileSystem fileSystem, ILogger<MoveOperate> logger)
        : base(fileSystem, logger)
    {
    }

    public override void Initialization(Guid guid)
    {
        CheckIsNotInitialized();
        throw new NotImplementedException("this Operate have not parameterless Initialization");
    }

    // see Initialization()
    public string Destination { get; set; } = null!;

    public void Initialization(string destination)
    {
        Initialization(Guid.NewGuid(), destination);
    }

    public void Initialization(Guid guid, string destination)
    {
        CheckIsNotInitialized();
        Guid = guid;
        Destination = destination;
        IsInitialized = true;
    }

    public override void Process(WatchingEventInfo info)
    {
        base.Process(info);

        // if running on windows, info.Path used separator may is '/'
        info.Path = info.Path.Replace('/', FileSystem.Path.DirectorySeparatorChar);
        var splitDest = info.Path.Split(FileSystem.Path.DirectorySeparatorChar);

        var dest = FileSystem.Directory.Exists(Destination)
            ? FileSystem.Path.Join(Destination, splitDest[^1].Length == 0 ? splitDest[^2] : splitDest[^1])
            : Destination;

        if (FileSystem.File.Exists(info.Path))
        {
            FileSystem.File.Move(info.Path, dest, true);

            Logger.LogTrace("MoveOperate Process on File branch, {Path} -> {Dest}", info.Path, dest);
        }
        else
        {
            FileSystem.Directory.Move(info.Path, dest);

            Logger.LogTrace("MoveOperate Process on Directory branch, {Path} -> {Dest}", info.Path, dest);
        }

        info.Path = dest;
    }
}