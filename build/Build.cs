namespace Queries.ContinuousIntegration;

// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using Candoumbe.Pipelines.Components;
using Candoumbe.Pipelines.Components.GitHub;
using Candoumbe.Pipelines.Components.NuGet;
using Candoumbe.Pipelines.Components.Workflows;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

using System;
using System.Collections.Generic;
using System.Linq;

[GitHubActions(
    "integration",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranchesIgnore = new[] { IHaveMainBranch.MainBranchName },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(IPack.Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
    ImportSecrets = new[]
    {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
        },
    OnPullRequestExcludePaths = new[]
    {
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        }
)]
[GitHubActions(
    "delivery",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = new[] { IHaveMainBranch.MainBranchName, IGitFlow.ReleaseBranch + "/*" },
    InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
    EnableGitHubToken = true,
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
    PublishArtifacts = true,
    ImportSecrets = new[]
    {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken)
        },
    OnPullRequestExcludePaths = new[]
    {
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        }
)]

//[GitHubActions("nightly", GitHubActionsImage.UbuntuLatest,
//    AutoGenerate = true,
//    FetchDepth = 0,
//    OnCronSchedule = "0 0 * * *",
//    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(IMutationTest.MutationTests), nameof(IPushNugetPackages.Pack) },
//    OnPushBranches = new[] { IHaveDevelopBranch.DevelopBranchName },
//    CacheKeyFiles = new[] {
//        "src/**/*.csproj",
//        "test/**/*.csproj",
//        "stryker-config.json",
//        "test/**/*/xunit.runner.json" },
//    EnableGitHubToken = true,
//    ImportSecrets = new[]
//    {
//        nameof(NugetApiKey),
//        nameof(IReportCoverage.CodecovToken),
//        nameof(IMutationTest.StrykerDashboardApiKey)
//    },
//    PublishArtifacts = true,
//    OnPullRequestExcludePaths = new[]
//    {
//        "docs/*",
//        "README.md",
//        "CHANGELOG.md",
//        "LICENSE"
//    }
//)]
[GitHubActions("nightly-manual", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    On = new[] { GitHubActionsTrigger.WorkflowDispatch },
    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(IMutationTest.MutationTests), nameof(IPushNugetPackages.Pack) },
    CacheKeyFiles = new[] {
            "src/**/*.csproj",
            "test/**/*.csproj",
            "stryker-config.json",
            "test/**/*/xunit.runner.json" },
    EnableGitHubToken = true,
    ImportSecrets = new[]
    {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
            nameof(IMutationTest.StrykerDashboardApiKey)
        },
    PublishArtifacts = true
)]

public class Build : NukeBuild,
    IHaveArtifacts,
    IHaveChangeLog,
    IHaveSolution,
    IHaveSourceDirectory,
    IHaveTestDirectory,
    IGitFlowWithPullRequest,
    IClean,
    IRestore,
    ICompile,
    IUnitTest,
    IMutationTest,
    IReportCoverage,
    IPack,
    IPushNugetPackages,
    ICreateGithubRelease
{
    [Parameter("API key used to publish artifacts to Nuget.org")]
    [Secret]
    public readonly string NugetApiKey;

    [Solution]
    [Required]
    public readonly Solution Solution;

    ///<inheritdoc/>
    Solution IHaveSolution.Solution => Solution;

    ///<inheritdoc/>
    public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IClean.DirectoriesToDelete => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/bin", "**/obj")
        .Concat(this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/bin", "**/obj"));

    ///<inheritdoc/>
    IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*Tests");

    ///<inheritdoc/>
    IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects
    {
        get
        {
            string[] projects = {
                "Queries.Core",
                "Queries.EntityFrameworkCore.Extensions",
                "Queries.EntityFrameworkCore.Extensions.SqlServer",
                "Queries.Renderers.MySQL",
                "Queries.Renderers.Neo4J",
                "Queries.Renderers.Postgres",
                "Queries.Renderers.Sqlite",
                "Queries.Renderers.SqlServer"
            };

            return projects
                .Select(projectName => new MutationProjectConfiguration(sourceProject: Solution.AllProjects.Single(csproj => string.Equals(csproj.Name, projectName, StringComparison.InvariantCultureIgnoreCase)),
                                                                        testProjects: Solution.AllProjects.Where(csproj => string.Equals(csproj.Name, $"{projectName}.Tests", StringComparison.InvariantCultureIgnoreCase)),
                                                                        configurationFile: this.Get<IHaveTestDirectory>().TestDirectory / $"{projectName}.Tests" / "stryker-config.json"))
                .Where(tuple => tuple.TestProjects.AtLeastOnce())
                .ToArray();
        }
    }

    ///<inheritdoc/>
    bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>().CodecovToken is not null;

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

    ///<inheritdoc/>
    IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations => new PushNugetPackageConfiguration[]
    {
            new NugetPushConfiguration(apiKey: NugetApiKey,
                                          source: new Uri("https://api.nuget.org/v3/index.json"),
                                          () => NugetApiKey is not null),
            new GitHubPushNugetConfiguration(githubToken: this.Get<IHaveGitHubRepository>().GitHubToken,
                                           source: new Uri("https://nukpg.github.com/"),
                                           () => this is ICreateGithubRelease && this.Get<ICreateGithubRelease>()?.GitHubToken is not null)
    };

    protected override void OnBuildCreated()
    {
        if (IsServerBuild)
        {
            EnvironmentInfo.SetVariable("DOTNET_ROLL_FORWARD", "LatestMajor");
        }
    }
}