using System.CommandLine;

namespace MonitorFileSystem.Client.Commands;

public class GlobalOptions
{
    public GlobalOptions(GrpcSettings settings)
    {
        var grpcAddress = string.IsNullOrEmpty(settings.Address)
            ? "https://localhost:5001"
            : settings.Address;
        
        GrpcAddress = new Option<string>(
            "--address",
            () =>
                string.IsNullOrEmpty(settings.Address)
                ? "https://localhost:5001"
                : settings.Address,
            "address of GrpcService")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        GrpcAddress.AddAlias("-d");
    }
    
    public Option<string> GrpcAddress { get; }
}