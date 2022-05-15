using System.CommandLine;
using MonitorFileSystem.Client.Configures;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands;

internal class GlobalOptions
{
    public GlobalOptions()
    {
        GrpcAddress = new Option<string>(
            "--address",
            () => Configure.GrpcSettings.AddressString,
            CommandTexts.GrpcAddress_OptionDescription);
        GrpcAddress.AddAlias("-d");
        GrpcAddress.AddValidator(result =>
        {
            var address = result.GetValueOrDefault<string>();
            if (address is not null)
            {
                Configure.GrpcSettings.AddressString = address;
            }
        });
    }

    public Option<string> GrpcAddress { get; }
}