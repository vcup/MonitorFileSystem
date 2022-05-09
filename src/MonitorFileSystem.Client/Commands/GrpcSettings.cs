namespace MonitorFileSystem.Client.Commands;

public class GrpcSettings
{
    public GrpcSettings(IConfiguration config)
    {
        Address = config.GetValue<string>("Grpc:Address");
    }
    
    public string Address { get; set; }
}