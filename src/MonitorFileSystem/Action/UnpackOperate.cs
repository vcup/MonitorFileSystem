using System.IO.Abstractions;
using System.IO.Compression;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public class UnpackOperate : OperateBase, IUnpackOperate
{
    public UnpackOperate(IFileSystem fileSystem, ILogger<UnpackOperate> logger) : base(fileSystem, logger)
    {
    }

    public string? Destination { get; set; }

    public override void Process(WatchingEventInfo info)
    {
        base.Process(info);
        
        if (FileSystem.Directory.Exists(info.Path))
        {
            return;
        }
        string dest = Destination ?? FileSystem.Path.GetDirectoryName(info.Path);

        using var stream = FileSystem.File.Open(info.Path, FileMode.Open, FileAccess.Read);
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        
        if (zipArchive.Entries.Count > 1)
        {
            dest = FileSystem.Path.Join(dest, FileSystem.Path.GetFileNameWithoutExtension(info.Path));
        }

        Task.WhenAll(zipArchive.Entries.Select(ExtractEntry));

        async Task ExtractEntry(ZipArchiveEntry entry)
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

            await using var entryStream = entry.Open();
            await using var file = FileSystem.FileStream.Create(path, FileMode.CreateNew, FileAccess.Write);
            
            await entryStream.CopyToAsync(file);
        }
    }
}