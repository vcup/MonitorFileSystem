using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MonitorFileSystem.Client.Configures;

internal static class Configure
{
    private const string ConfigFilePath = "./config.yaml";
    private static readonly Settings Settings;
    
    static Configure()
    {
        try
        {
            using var configFile = File.OpenText(ConfigFilePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            
            Settings = deserializer.Deserialize<Settings>(configFile);
        }
        catch (FileNotFoundException)
        {
            Settings = new Settings();
            using var configFile = File.CreateText(ConfigFilePath);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            serializer.Serialize(configFile, Settings);
        }
    }

    public static GrpcSettings GrpcSettings => Settings.GrpcSettings;
}