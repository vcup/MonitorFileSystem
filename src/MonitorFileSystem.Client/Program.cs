using System.CommandLine;

var rootCommand = new RootCommand("commandline client of monitor file system service");

return await rootCommand.InvokeAsync(args);