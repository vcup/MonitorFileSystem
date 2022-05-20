using Microsoft.Extensions.Configuration;

namespace MonitorFileSystem.Client.Configures;

internal static class Configure
{
    private static readonly Settings Settings;
    
    static Configure()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddYamlFile("./config.yaml", true)
            .Build();
        Settings = new Settings
        {
            GrpcSettings = new GrpcSettings
            {
                AddressString = config["grpc:address"] ?? "https://localhost:5001"
            }
        };
    }

    public static GrpcSettings GrpcSettings => Settings.GrpcSettings;
}