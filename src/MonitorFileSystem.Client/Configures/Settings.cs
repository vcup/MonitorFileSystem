using YamlDotNet.Serialization;

namespace MonitorFileSystem.Client.Configures;

internal class Settings
{
    public Settings()
    {
        GrpcSettings = new GrpcSettings
        {
            AddressString = "https://localhost:5001"
        };
    }

    [YamlMember(Alias = "Grpc", Description = "Remote grpc service Settings")]
    public GrpcSettings GrpcSettings { get; set; }
}