using System.ComponentModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorFileSystem.Action;
using MonitorFileSystem.Common;
using MonitorFileSystem.Extensions;
using MonitorFileSystem.Monitor;
using NUnit.Framework;

namespace MonitorFileSystem.Tests;

public class CommandOperateTests : InitializableBaseTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Provider = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddCommandOperate()
                    .AddScoped<IInitializable, CommandOperate>();
            })
            .Build()
            .Services;
    }

    [TestCase("{0}", CommandOperateArgument.Name, "file")]
    [TestCase("{0}", CommandOperateArgument.Path, "/dir/file")]
    [TestCase("{0}", CommandOperateArgument.OldName, "old_file")]
    [TestCase("{0}", CommandOperateArgument.OldPath, "/old/dir/old_file")]
    public void StartInfo_Process_SetupCommandLine(string template, CommandOperateArgument argument,
        string expectedCommandLine)
    {
        var scoped = Provider.CreateScope();
        
        var startInfoField = typeof(CommandOperate).GetField("_startInfo", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(startInfoField);
        var filesystem = scoped.ServiceProvider.GetRequiredService<IFileSystem>();
        var operate = scoped.ServiceProvider.GetRequiredService<ICommandOperate>();
        operate.Initialization(template, argument);
        var info = new WatchingEventInfo
        {
            Path = "/dir/file",
            OldPath = "/old/dir/old_file"
        };

        Assert.Throws<Win32Exception>(() => operate.Process(info));
        var startInfo = startInfoField!.GetValue(operate) as ProcessStartInfo;
        
        Assert.IsNotNull(startInfo);
        Assert.AreEqual(startInfo!.Arguments, expectedCommandLine);
        Assert.AreEqual(filesystem.Path.GetDirectoryName(info.Path), startInfo.WorkingDirectory);
        Assert.IsFalse(startInfo.UseShellExecute);
    }

    [TestCase("/dir/", "dir")]
    [TestCase("/dir/file", "file")]
    [TestCase("file/", "file")]
    [TestCase(null, null)]
    public void GetNameOfPath_UseReflection_ReturnFileOrDirectoryName(string? path, string? name)
    {
        var method = typeof(CommandOperate).GetMethod("GetNameOfPath", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(method);
        var result = method!.Invoke(typeof(CommandOperate), new object?[] { path }) as string;

        Assert.AreEqual(name, result);
    }

    [Test]
    public void Process_EchoMessage_ShouldOutputMessage()
    {
        var scoped = Provider.CreateScope();

        var operate = scoped.ServiceProvider.GetRequiredService<ICommandOperate>();
#if Linux
        operate.Initialization("echo hello");
#endif
        var info = new WatchingEventInfo();

        operate.Process(info);

        Assert.AreEqual("hello", operate.CommandOutput?.ReplaceLineEndings(""));
    }
}