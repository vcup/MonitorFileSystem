namespace MonitorFileSystem.Client;

public class GrpcSettings
{
    public GrpcSettings(IConfiguration config)
    {
        Address = config.GetValue<string>("Grpc:Address") ?? "https://localhost:5001";
    }
    
    public string Address { get; set; }
}