using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public class MoveOperate : OperateBase, IMoveOperate
{
    // see Initialization()
    private string _destination = null!;

    public MoveOperate(IFileSystem fileSystem, ILogger<MoveOperate> logger)
        : base(fileSystem, logger)
    {
    }

    public override void Initialization(Guid guid)
    {
        CheckIsNotInitialized();
        throw new NotImplementedException("this Operate have not parameterless Initialization");
    }

    public void Initialization(string destination)
    {
        Initialization(Guid.NewGuid(), destination);
    }

    public void Initialization(Guid guid, string destination)
    {
        CheckIsNotInitialized();
        Guid = guid;
        _destination = destination;
        IsInitialized = true;
    }
    
    public override void Process(WatchingEventInfo info)
    {
        base.Process(info);
        var dest = FileSystem.Directory.Exists(_destination)
            ? FileSystem.Path.Join(_destination, info.Path)
            : _destination;
             
        if (FileSystem.File.Exists(info.Path))
        {
           
            FileSystem.File.Move(info.Path, _destination, true);
            // if destIsFile is null, dest path also is a File. because a file is Moved to dest path
            
            Logger.LogTrace("MoveOperate Process on File branch, {path} -> {dest}", info.Path, dest);
        }
        else
        {
            FileSystem.Directory.Move(info.Path, dest);
            // if destIsFile is null, dest path also is a directory. because a directory is Moved to dest path
            
            Logger.LogTrace("MoveOperate Process on Directory branch, {path} -> {dest}", info.Path, dest);
        }

        info.Path = dest;
    }
}
