namespace MonitorFileSystem.Client.Configures;

internal static class Configure
{
    static Configure()
    {
        GrpcSettings = new GrpcSettings();
    }
    
    public static GrpcSettings GrpcSettings { get; }
}