namespace MonitorFileSystem.Client.Grpc;

public class GrpcSettings
{
    // will set on AddressString.set
    private Uri _uri = null!;
    
    public GrpcSettings()
    {
        AddressString = "https://localhost:5001";
    }

    public string AddressString
    {
        get => _uri.ToString();
        set
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                _uri = uri;
                return;
            }

            throw new ArgumentException("{Name} is a invalid address", nameof(value));
        }
    }

    public Uri Address => _uri;
}