﻿using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public class MoveOperate : OperateBase
{
    // see Initialization()
    private string _destination = null!;

    public MoveOperate(ILogger<MoveOperate> logger)
        : base(logger)
    {
    }

    public override void Initialization(params object[] parameters)
    {
        if (parameters.Length is 0)
        {
            throw new ArgumentException("Parameters must have a string of destination path");
        }
        var startIndex = parameters[0] is IFileSystem ? 1 : 0;
        _destination = (string)parameters[startIndex];
        base.Initialization(parameters);
    }

    public override void Process(WatchingEventInfo info)
    {
        CheckIsInitialized();
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
