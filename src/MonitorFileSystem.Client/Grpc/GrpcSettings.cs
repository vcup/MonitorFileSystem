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
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                return; // can throw some
            }

            _uri = uri;
        }
    }

    public Uri Address => _uri;
}