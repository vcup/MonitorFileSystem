﻿using System.CommandLine;
using MonitorFileSystem.Client.Commands;

var rootCommand = new RootCommand("commandline client of monitor file system service");

rootCommand.AddGlobalOptions()
    .AddWatchCommands()
    .AddOperateCommands()
    ;

return await rootCommand.InvokeAsync(args);