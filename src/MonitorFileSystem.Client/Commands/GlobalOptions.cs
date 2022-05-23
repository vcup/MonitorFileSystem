using System.CommandLine;
using MonitorFileSystem.Client.Resources;

namespace MonitorFileSystem.Client.Commands;

internal static class GlobalOptions
{
    static GlobalOptions()
    {
        GrpcAddress = new(
            "--address",
            CommandTexts.GrpcAddress_OptionDescription);
        GrpcAddress.AddAlias("-d");

        ConfigPath = new(new[] { "--config-path", "--conf", "-c" })
        {
            Arity = ArgumentArity.ZeroOrOne
        };
    }

    public static Option<string> GrpcAddress { get; }
    public static Option<string> ConfigPath { get; }
}