namespace MonitorFileSystem.Client.Grpc;

public class GrpcSettings
{
    public GrpcSettings()
    {
        Address = "https://localhost:5001";
    }
    
    public string Address { get; set; }
}