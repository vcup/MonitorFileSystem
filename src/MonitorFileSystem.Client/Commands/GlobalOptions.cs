using System.CommandLine;
using System.Runtime.InteropServices.ComTypes;

namespace MonitorFileSystem.Client.Commands;

public class GlobalOptions
{
    public GlobalOptions(GrpcSettings settings)
    {
        GrpcAddress = new Option<string>(
            "--address",
            () => settings.Address,
            "address of GrpcService");
        GrpcAddress.AddAlias("-d");
        GrpcAddress.AddValidator(result =>
        {
            var address = result.GetValueOrDefault<string>();
            if (Uri.TryCreate(address, UriKind.Absolute, out var uri) && !uri.IsFile)
            {
                settings.Address = address;
            }
        });
    }
    
    public Option<string> GrpcAddress { get; }
}