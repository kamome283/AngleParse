// ReSharper disable RedundantUsingDirective

using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable NotAccessedField.Local

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;

    [Solution] readonly Solution Solution;

    Project MainProject => Solution.GetProject("AngleParse");
    Project TestProject => Solution.GetProject("AngleParse.Test");
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath ModuleDefinition => RootDirectory / "AngleParse.psd1";

    Target CleanPublish => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.GlobDirectories("*").ForEach(DeleteDirectory);
            ArtifactsDirectory.GlobFiles("*").ForEach(DeleteFile);
        });

    Target Clean => _ => _
        .Before(Restore)
        .DependsOn(CleanPublish)
        .Executes(() =>
        {
            Solution.Directory.GlobDirectories("AngleParse*/bin", "AngleParse*/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target UnitTest => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(UnitTest)
        .DependsOn(CleanPublish)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(MainProject)
                .SetConfiguration(Configuration.Release)
                .SetOutput(ArtifactsDirectory)
                .EnableNoRestore());

            CopyFile(ModuleDefinition, ArtifactsDirectory / "AngleParse.psd1");
        });

    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);
}