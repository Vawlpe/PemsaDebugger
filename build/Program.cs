using Cake.Core;
using Cake.Frosting;
using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Run;
using Cake.CMake;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }
    public string Framework { get; set; }
    public string Project { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Debug");
        Framework = context.Argument("framework", "net6.0");
        Project = context.Argument("project", "PemsaDebugger");
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory($"../src/{context.Project}/bin");
        context.CleanDirectory($"../src/{context.Project}/obj");
    }
}

[TaskName("Build Native PEMSA lib")]
[IsDependentOn(typeof(CleanTask))]
public sealed class BuildNativePemsaTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        CMakeAliases.CMake(context, new CMakeSettings
        {
            SourcePath = "../src/pemsa-pinvoke",
            OutputPath = $"../src/{context.Project}/bin/native/pemsa-invoke"
        });
        CMakeAliases.CMakeBuild(context, new CMakeBuildSettings 
        {
            BinaryPath = $"../src/{context.Project}/bin/native/pemsa-invoke",
            CleanFirst = true,
        });
        context.CopyFiles("../src/pemsa-pinvoke/dist/*", $"../src/{context.Project}/bin/native");
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(BuildNativePemsaTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CopyFiles("../src/ImGui.NET/deps/cimgui/" +
            (context.IsRunningOnLinux()    ? "linux-x64/*"
            : context.IsRunningOnWindows() ? "win-x64/*"
            : "osx/*"
            ), $"../src/{context.Project}/bin/native/"
        );
        context.DotNetBuild($"../src/{context.Project}/{context.Project}.csproj", new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration,
            Framework = context.Framework
        });
        context.CopyFiles($"../src/{context.Project}/bin/native/*", $"../src/{context.Project}/bin/{context.MsBuildConfiguration}/{context.Framework}");
        context.CopyDirectory("../carts", $"../src/{context.Project}/bin/{context.MsBuildConfiguration}/{context.Framework}/carts");
        
    }
}

// [TaskName("Run")]
// [IsDependentOn(typeof(BuildTask))]
// public sealed class RunTask : FrostingTask<BuildContext>
// {
//     public override void Run(BuildContext context)
//     {
//         context.DotNetRun($"../src/{context.Project}/{context.Project}.csproj", new DotNetRunSettings
//         {
//             Configuration = context.MsBuildConfiguration,
//             Framework = context.Framework,
//             NoBuild = true,
//             NoRestore = true
//         });
//     }
// }

// [IsDependentOn(typeof(RunTask))]
[IsDependentOn(typeof(BuildTask))]
public sealed class Default : FrostingTask
{
}
