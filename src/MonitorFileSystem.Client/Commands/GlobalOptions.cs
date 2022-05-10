using System.CommandLine;
using MonitorFileSystem.Client.Grpc;

namespace MonitorFileSystem.Client.Commands;

internal class GlobalOptions
{
    public GlobalOptions()
    {
        GrpcAddress = new Option<string>(
            "--address",
            () => GrpcUnits.Settings.AddressString,
            "address of GrpcService");
        GrpcAddress.AddAlias("-d");
        GrpcAddress.AddValidator(result =>
        {
            var address = result.GetValueOrDefault<string>();
            if (address is not null)
            {
                GrpcUnits.Settings.AddressString = address;
            }
        });
    }
    
    public Option<string> GrpcAddress { get; }
}