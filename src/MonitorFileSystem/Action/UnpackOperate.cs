using System.IO.Abstractions;
using System.IO.Compression;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public class UnpackOperate : OperateBase, IUnpackOperate
{
    private string? _destination;
    private bool _ignoreDirectory;
    
    public UnpackOperate(IFileSystem fileSystem, ILogger<UnpackOperate> logger) : base(fileSystem, logger)
    {
    }

    public void Initialization(bool ignoreDirectory)
    {
        Initialization(null, ignoreDirectory);
    }

    public void Initialization(string? destination)
    {
        Initialization(destination, false);
    }

    public void Initialization(string? destination, bool ignoreDirectory)
    {
        CheckIsNotInitialized();
        _destination = destination;
        _ignoreDirectory = ignoreDirectory;
        IsInitialized = true;
    }

    public override void Process(WatchingEventInfo info)
    {
        base.Process(info);

        if (_ignoreDirectory)
        {
            return;
        }
        
        if (FileSystem.Directory.Exists(info.Path))
        {
            throw new InvalidOperationException("Path is a Directory");
        }
        string dest = _destination ?? FileSystem.Path.GetDirectoryName(info.Path);

        using var stream = FileSystem.File.Open(info.Path, FileMode.Open, FileAccess.Read);
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        
        if (zipArchive.Entries.Count > 1)
        {
            dest = FileSystem.Path.Combine(dest, FileSystem.Path.GetFileNameWithoutExtension(info.Path));
        }
        
        foreach (var entry in zipArchive.Entries)
        {
            ExtractEntry(entry);
        }
        
        void ExtractEntry(ZipArchiveEntry entry)
        { 
            var path = FileSystem.Path.Combine(dest, entry.FullName); 
            var directory = FileSystem.Path.GetDirectoryName(path);
            if (!FileSystem.Directory.Exists(directory))
            {
                FileSystem.DirectoryInfo.FromDirectoryName(directory).Create();
            }

            if (FileSystem.Directory.Exists(path))
            {
                return;
            }
            using var entryStream = entry.Open();
            using var file = FileSystem.FileStream.Create(path, FileMode.CreateNew, FileAccess.Write);
            
            entryStream.CopyTo(file);
        }
    }

}