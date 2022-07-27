using Microsoft.Extensions.Configuration;

namespace MonitorFileSystem.Client.Configures;

internal static class Configure
{
    private static Settings _settings = null!;

    static Configure()
    {
    }

    public static void SetupSettings(IConfiguration config)
    {
        _settings = new Settings
        {
            GrpcSettings = new GrpcSettings
            {
                AddressString = config["grpc:address"] ?? "https://localhost:5001"
            }
        };
    }

    public static GrpcSettings GrpcSettings => _settings.GrpcSettings;
}