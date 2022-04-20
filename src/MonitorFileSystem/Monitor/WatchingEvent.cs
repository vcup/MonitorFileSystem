namespace MonitorFileSystem.Monitor;

[Flags]
internal enum WatchingEvent
{
    None          = 0b0000_0000_0000_0000,
    Created       = 0b0000_0000_0000_0001,
    Deleted       = 0b0000_0000_0000_0010,
    Renamed       = 0b0000_0000_0000_0100,
    Changed       = 0b0001_0111_1111_1000,
    FileName      = 0b0000_0000_0001_0000, // >>4 : 1
    DirectoryName = 0b0000_0000_0010_0000, // >>4 : 2
    Attributes    = 0b0000_0000_0100_0000, // >>4 : 4
    Size          = 0b0000_0000_1000_0000, // >>4 : 8
    LastWrite     = 0b0000_0001_0000_0000, // >>4 : 16
    LastAccess    = 0b0000_0010_0000_0000, // >>4 : 32
    CreationTime  = 0b0000_0100_0000_0000, // >>4 : 64
    Security      = 0b0001_0000_0000_0000, // >>4 : 256
}
