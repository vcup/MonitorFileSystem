using System.CommandLine;
using MonitorFileSystem.Grpc.ProtocolBuffers;

namespace MonitorFileSystem.Client.Commands.OperateCommands;

public class UpdateCommandOperateCommand : Command
{
    public UpdateCommandOperateCommand()
        : base("command")
    {
        var guid = new Argument<string>
        {
            Name = "guid"
        };

        var template = new Argument<string?>
        {
            Name = "template"
        };
        template.SetDefaultValue(null);

        var arguments = new Argument<IEnumerable<CommandOperateArgument>?>
        {
            Name = "arguments"
        };
        arguments.SetDefaultValue(null);

        var description = new Argument<string?>
        {
            Name = "description"
        };
        description.SetDefaultValue(null);
        
        AddArgument(guid);
        AddArgument(template);
        AddArgument(arguments);
        AddArgument(description);
        
        this.SetHandler<string, string?, IEnumerable<CommandOperateArgument>?, string?>(Update, guid, template, arguments, description);
    }

    private static async Task Update(string guid,
        string? template, IEnumerable<CommandOperateArgument>? arguments, string? description)
    {
        var request = new UpdateCommandOperateRequest
        {
            Guid = guid
        };

        if (template is not null)
        {
            request.CommandTemplate = template;
        }

        if (arguments is not null)
        {
            request.Arguments.AddRange(arguments);
        }

        if (description is not null)
        {
            request.Description = description;
        }

        await GrpcUnits.ActionManagementClient.UpdateCommandOperateAsync(request);
    }
}