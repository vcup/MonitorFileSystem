using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

internal class AddCommandOperateCommand : Command
{
    public AddCommandOperateCommand()
        : base("command")
    {
        var template = new Argument<string>
        {
            Name = "template",
        };

        var argument = new Argument<IEnumerable<CommandOperateArgument>>
        {
            Name = "arguments",
            Arity = ArgumentArity.ZeroOrMore
        };

        var description = new Argument<string?>
        {
            Name = "description"
        };
        description.SetDefaultValue(null);
        
        AddArgument(template);
        AddArgument(description);
        AddArgument(argument);
        
        this.SetHandler<string, IEnumerable<CommandOperateArgument>, string?>(Create, template, argument, description);
    }

    private static async Task Create(string template, IEnumerable<CommandOperateArgument> arguments, string? description)
    {
        var request = new CommandOperateRequest
        {
            CommandTemplate = template
        };

        if (description is not null)
        {
            request.Description = description;
        }

        request.Arguments.AddRange(arguments);

        var response = await GrpcUnits.ActionManagementClient.CreateCommandOperateAsync(request);
        
        Console.WriteLine(response.Guid);
    }
}