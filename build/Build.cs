using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

namespace Queries.Pipelines
{
    [AzurePipelines(
        AzurePipelinesImage.WindowsLatest,
        InvokedTargets = new[] { nameof(Pack) },
        NonEntryTargets = new[] { nameof(Restore) },
        ExcludedTargets = new [] { nameof(Clean) },
        PullRequestsAutoCancel = true,
        PullRequestsBranchesInclude = new[] { MainBranch },
        TriggerBranchesInclude = new[] {
            MainBranch,
            FeatureBranch + "/*",
            SupportBranch + "/*",
            HotfixBranch + "/*"
        },
        TriggerPathsExclude = new[]
        {
        "docs/*",
        "README.md"
        }
    )]
    [CheckBuildProjectConfigurations]
    [UnsetVisualStudioEnvironmentVariables]
    public class Build : NukeBuild
    {
        public static int Main() => Execute<Build>(x => x.Pack);

        [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
        public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

        [Parameter("Indicates wheter to restore nuget in interactive mode - Default is false")]
        public readonly bool Interactive = false;

        [Solution] public readonly Solution Solution;
        [GitRepository] public readonly GitRepository GitRepository;
        [GitVersion] public readonly GitVersion GitVersion;

        [CI] public readonly AzurePipelines AzurePipelines;

        [Partition(4)] public readonly Partition TestPartition;

        public AbsolutePath SourceDirectory => RootDirectory / "src";

        public AbsolutePath TestDirectory => RootDirectory / "test";

        public AbsolutePath OutputDirectory => RootDirectory / "output";

        public AbsolutePath CoverageReportDirectory => OutputDirectory / "coverage-report";

        public AbsolutePath TestResultDirectory => OutputDirectory / "tests-results";

        public AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";

        public const string MainBranch = "main";
        public const string FeatureBranch = "feature";
        public const string HotfixBranch = "hotfix";
        public const string ReleaseBranch = "release";
        public const string SupportBranch = "support";

        public Target Clean => _ => _
            .Before(Restore)
            .Executes(() =>
            {
                SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                TestDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                EnsureCleanDirectory(OutputDirectory);
            });

        public Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                DotNetRestore(s => s
                    .SetProjectFile(Solution)
                    .SetIgnoreFailedSources(true)
                    .SetDisableParallel(false)
                    .When(IsLocalBuild && Interactive, _ => _.SetProperty("NugetInteractive", IsLocalBuild && Interactive))
                );
            });

        public Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                DotNetBuild(s => s
                    .SetNoRestore(InvokedTargets.Contains(Restore))
                    .SetConfiguration(Configuration)
                    .SetProjectFile(Solution)
                    .SetAssemblyVersion(GitVersion.AssemblySemVer)
                    .SetFileVersion(GitVersion.AssemblySemFileVer)
                    .SetInformationalVersion(GitVersion.InformationalVersion)
                    );
            });

        public Target Tests => _ => _
            .DependsOn(Compile)
            .Description("Run unit tests and collect code")
            .Produces(TestResultDirectory / "*.trx")
            .Produces(TestResultDirectory / "*.xml")
            .Partition(() => TestPartition)
            .Executes(() =>
            {
                IEnumerable<Project> projects = Solution.GetProjects("*.Tests");
                IEnumerable<Project> testsProjects = TestPartition.GetCurrent(projects);

                testsProjects.ForEach(project => Info(project));

                if (testsProjects.Any())
                {

                    Info("Before running 'dotnet test'");
                    DotNetTest(s => s
                        .SetConfiguration(Configuration)
                        .EnableCollectCoverage()
                        .EnableUseSourceLink()
                        .SetNoBuild(InvokedTargets.Contains(Compile))
                        .SetResultsDirectory(TestResultDirectory)
                        .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                        .AddProperty("ExcludeByAttribute", "Obsolete")
                        .CombineWith(testsProjects, (cs, project) => cs.SetProjectFile(project)
                            .CombineWith(project.GetTargetFrameworks(), (setting, framework) => setting
                                .SetFramework(framework)
                                .SetLogger($"trx;LogFileName={project.Name}.{framework}.trx")
                                .SetCoverletOutput(TestResultDirectory / $"{project.Name}.{framework}.xml"))
                            )
                    );
                    Trace("After running 'dotnet test'");

                    Trace("Before publishing test results");
                    TestResultDirectory.GlobFiles("*.trx")
                                    .ForEach(testFileResult => AzurePipelines?.PublishTestResults(type: AzurePipelinesTestResultsType.VSTest,
                                                                                                    title: $"{Path.GetFileNameWithoutExtension(testFileResult)} ({AzurePipelines.StageDisplayName})",
                                                                                                    files: new string[] { testFileResult })
                    );
                    Trace("After publishing test results");


                    // TODO Move this to a separate "coverage" target once https://github.com/nuke-build/nuke/issues/562 is solved !
                    Trace("Before reporting code coverage");
                    ReportGenerator(_ => _
                                .SetFramework("net5.0")
                                .SetReports(TestResultDirectory / "*.xml")
                                .SetReportTypes(ReportTypes.Badges, ReportTypes.HtmlChart, ReportTypes.HtmlInline_AzurePipelines_Dark)
                                .SetTargetDirectory(CoverageReportDirectory)
                            );
                    Trace("After reporting code coverage");

                    Trace("Before publishing code coverage");
                    TestResultDirectory.GlobFiles("*.xml")
                                    .ForEach(file => AzurePipelines?.PublishCodeCoverage(coverageTool: AzurePipelinesCodeCoverageToolType.Cobertura,
                                                                                            summaryFile: file,
                                                                                            reportDirectory: CoverageReportDirectory));
                    Trace("After publishing code coverage");
                    Info("Fin d'exécution des tests");

                }
            });

        public Target Pack => _ => _
            .DependsOn(Tests, Compile)
            .Consumes(Compile)
            .Produces(ArtifactsDirectory / "*.nupkg")
            .Executes(() =>
            {
                DotNetPack(s => s
                    .EnableIncludeSource()
                    .EnableIncludeSymbols()
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetProject(Solution)
                    .SetConfiguration(Configuration)
                    .SetAssemblyVersion(GitVersion.AssemblySemVer)
                    .SetFileVersion(GitVersion.AssemblySemFileVer)
                    .SetInformationalVersion(GitVersion.InformationalVersion)
                    .SetVersion(GitVersion.NuGetVersion)
                );
            });

        protected override void OnTargetStart(string target)
        {
            Info($"Starting '{target}' task");
        }

        protected override void OnTargetExecuted(string target)
        {
            Info($"'{target}' task finished");
        }
    }
}