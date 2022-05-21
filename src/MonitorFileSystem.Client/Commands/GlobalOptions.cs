using System.CommandLine;
using MonitorFileSystem.Client.Configures;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands;

internal static class GlobalOptions
{
    static GlobalOptions()
    {
        GrpcAddress = new(
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

        ConfigPath = new(new []{"--config-path", "--conf", "-c"})
        {
            Arity = ArgumentArity.ZeroOrOne
        };
    }

    public static Option<string> GrpcAddress { get; }
    public static Option<string> ConfigPath { get; }
}