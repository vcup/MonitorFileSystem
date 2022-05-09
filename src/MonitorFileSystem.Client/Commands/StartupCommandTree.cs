using System.CommandLine;
using MonitorFileSystem.Client.Commands.WatchCommands;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands;

public static class StartupCommandTree
{
    public static IServiceCollection AddCommandTree(this IServiceCollection services)
    {
        return services.AddScoped(provider =>
        {
            var args = provider.GetRequiredService<CommandLineArguments>();
            var globalOptions = provider.GetRequiredService<GlobalOptions>();
            var rootCommand = new RootCommand();
            foreach (var property in typeof(GlobalOptions).GetProperties())
            {
                var value = property.GetValue(globalOptions);
                if (value is Option option)
                {
                    rootCommand.Add(option);
                }
            }
            
            rootCommand.AddCommand(new WatchCommand(provider.GetRequiredService<MonitorManagement.MonitorManagementClient>()));

            return new CommandTree(rootCommand, args.Arguments);
        });
    }
}