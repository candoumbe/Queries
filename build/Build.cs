namespace Queries.ContinuousIntegration;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Codecov;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.GitReleaseManager;

using System;
using System.IO;
using System.Linq;

using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Tools.Codecov.CodecovTasks;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Utilities;
using System.Collections.Generic;
using static Serilog.Log;

[GitHubActions(
    "integration",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranchesIgnore = new[] { MainBranchName, ReleaseBranchPrefix + "/*" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Tests), nameof(Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "test/**/*.csproj", },
    ImportSecrets = new[]
    {
        nameof(NugetApiKey),
        nameof(CodecovToken)
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
    OnPushBranches = new[] { MainBranchName },
    InvokedTargets = new[] { nameof(Tests), nameof(Publish), nameof(AddGithubRelease) },
    EnableGitHubToken = true,
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "test/**/*.csproj", },
    PublishArtifacts = true,
    ImportSecrets = new[]
    {
        nameof(NugetApiKey),
        nameof(CodecovToken)
    },
    OnPullRequestExcludePaths = new[]
    {
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    }
)]

[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
[HandleVisualStudioDebugging]
public class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Indicates wheter to restore nuget in interactive mode - Default is false")]
    public readonly bool Interactive = false;

    [Parameter("Generic name placeholder. Can be used wherever a name is required")]
    public readonly string Name;

    [Parameter]
    [Secret]
    public readonly string GitHubToken;

    [Parameter]
    [Secret]
    public readonly string CodecovToken;

    [Required] [Solution] public readonly Solution Solution;
    [Required] [GitRepository] public readonly GitRepository GitRepository;
    [Required] [GitVersion(Framework = "net5.0")] public readonly GitVersion GitVersion;

    [CI] public readonly AzurePipelines AzurePipelines;
    [CI] public readonly GitHubActions GitHubActions;

    [Partition(3)] public readonly Partition TestPartition;

    /// <summary>
    /// Directory of source code projects
    /// </summary>
    public AbsolutePath SourceDirectory => RootDirectory / "src";

    /// <summary>
    /// Directory of test projects
    /// </summary>
    public AbsolutePath TestDirectory => RootDirectory / "test";

    /// <summary>
    /// Directory where to store all output builds output
    /// </summary>
    public AbsolutePath OutputDirectory => RootDirectory / "output";

    /// <summary>
    /// Directory where to pu
    /// </summary>
    public AbsolutePath CoverageReportDirectory => OutputDirectory / "coverage-report";

    /// <summary>
    /// Directory where to publish all test results
    /// </summary>
    public AbsolutePath TestResultDirectory => OutputDirectory / "tests-results";

    /// <summary>
    /// Directory where to publish all artifacts
    /// </summary>
    public AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";

    /// <summary>
    /// Directory where to publish converage history report
    /// </summary>
    public AbsolutePath CoverageReportHistoryDirectory => OutputDirectory / "coverage-history";

    /// <summary>
    /// Directory where to publish benchmark results.
    /// </summary>
    public AbsolutePath BenchmarkDirectory => OutputDirectory / "benchmarks";

    public const string MainBranchName = "main";

    public const string DevelopBranch = "develop";

    public const string FeatureBranchPrefix = "feature";

    public const string HotfixBranchPrefix = "hotfix";

    public const string ColdfixBranchPrefix = "coldfix";

    public const string ReleaseBranchPrefix = "release";

    [Parameter("Indicates if any changes should be stashed automatically prior to switching branch (Default : true)")]
    public readonly bool AutoStash = true;

    public Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(action: DeleteDirectory);
            TestDirectory.GlobDirectories("**/bin", "**/obj").ForEach(action: DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(CoverageReportDirectory);
            EnsureExistingDirectory(CoverageReportHistoryDirectory);
        });

    public Target Restore => _ => _
        .After(Clean)
        .Executes(() =>
        {
            DotNetToolRestore();
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
                .SetNoRestore(InvokedTargets.Contains(Restore) || SkippedTargets.Contains(Restore))
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
            );
        });

    public Target Tests => _ => _
        .DependsOn(Compile)
        .Description("Run unit tests and collect code coverage")
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Triggers(ReportCoverage)
        .Executes(() =>
        {
            IEnumerable<Project> projects = Solution.GetProjects("*.Tests");
            IEnumerable<Project> testsProjects = TestPartition.GetCurrent(projects);

            testsProjects.ForEach(action: project => Verbose(project));

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .ResetVerbosity()
                .EnableCollectCoverage()
                .SetResultsDirectory(TestResultDirectory)
                .SetCoverletOutputFormat(CoverletOutputFormat.lcov)
                .When(IsServerBuild, _ => _.EnableUseSourceLink())
                .AddProperty("ExcludeByAttribute", "Obsolete")
                .SetExcludeByFile("*.Generated.cs")
                .CombineWith(testsProjects, (cs, project) => cs.SetProjectFile(project)
                                                               .CombineWith(project.GetTargetFrameworks(), (setting, framework) => setting.SetFramework(framework)
                                                                                                                                          .AddLoggers($"trx;LogFileName={project.Name}.trx")
                                                                                                                                          .SetCoverletOutput(TestResultDirectory / $"{project.Name}.{framework}.xml"))),
                                                                                                                                          completeOnFailure: true
                );

            TestResultDirectory.GlobFiles("*.trx")
                                    .ForEach(action: testFileResult => AzurePipelines?.PublishTestResults(type: AzurePipelinesTestResultsType.VSTest,
                                                                                                    title: $"{Path.GetFileNameWithoutExtension(testFileResult)} ({AzurePipelines.StageDisplayName})",
                                                                                                    files: new string[] { testFileResult })
                    );

            TestResultDirectory.GlobFiles("*.xml")
                            .ForEach(action: file => AzurePipelines?.PublishCodeCoverage(coverageTool: AzurePipelinesCodeCoverageToolType.Cobertura,
                                                                                    summaryFile: file,
                                                                                    reportDirectory: CoverageReportDirectory));

        });

    public Target ReportCoverage => _ => _
        .DependsOn(Tests)
        .After(Tests)
        .OnlyWhenDynamic(() => IsServerBuild || CodecovToken != null)
        .Consumes(Tests, TestResultDirectory / "*.xml")
        .Produces(CoverageReportDirectory / "*.xml")
        .Produces(CoverageReportHistoryDirectory / "*.xml")
        .Executes(() =>
        {
            ReportGenerator(_ => _
                   .SetFramework("net5.0")
                   .SetReports(TestResultDirectory / "*.xml")
                   .SetReportTypes(ReportTypes.Badges, ReportTypes.HtmlChart, ReportTypes.HtmlInline_AzurePipelines_Dark)
                   .SetTargetDirectory(CoverageReportDirectory)
                   .SetHistoryDirectory(CoverageReportHistoryDirectory)
                   .SetTag(GitRepository.Commit)
               );

            Codecov(s => s
                .SetFiles(TestResultDirectory.GlobFiles("*.xml").Select(x => x.ToString()))
                .SetToken(CodecovToken)
                .SetBranch(GitRepository.Branch)
                .SetSha(GitRepository.Commit)
                .SetBuild(GitVersion.FullSemVer)
                .SetFramework("netcoreapp3.0")
            );
        });

    public Target Pack => _ => _
        .DependsOn(Tests, Compile)
        .Consumes(Compile)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Produces(ArtifactsDirectory / "*.snupkg")
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
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                .SetPackageReleaseNotes(GetNuGetReleaseNotes(ChangeLogFile, GitRepository))
                .SetRepositoryType("git")
                .SetRepositoryUrl(GitRepository.HttpsUrl)
            );
        });

    private AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

    #region Git flow section

    public Target Changelog => _ => _
        .Requires(() => IsLocalBuild)
        .OnlyWhenStatic(() => GitRepository.IsOnReleaseBranch() || GitRepository.IsOnHotfixBranch())
        .Description("Finalizes the change log so that its up to date for the release. ")
        .Executes(() =>
        {
            FinalizeChangelog(ChangeLogFile, GitVersion.MajorMinorPatch, GitRepository);
            Information("Please review CHANGELOG.md ({ChangeLogFile}) and press 'Y' to validate (any other key will cancel changes)...", ChangeLogFile);
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.Y)
            {
                Git($"add {ChangeLogFile}");
                Git($"commit -m \"Finalize {Path.GetFileName(ChangeLogFile)} for {GitVersion.MajorMinorPatch}\"");
            }
        });

    public Target Feature => _ => _
        .Description($"Starts a new feature development by creating the associated branch {FeatureBranchPrefix}/{{feature-name}} from {DevelopBranch}")
        .Requires(() => IsLocalBuild)
        .Requires(() => !GitRepository.IsOnFeatureBranch() || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            if (!GitRepository.IsOnFeatureBranch())
            {
                Information("Enter the name of the feature. It will be used as the name of the feature/branch (leave empty to exit) :");
                AskBranchNameAndSwitchToIt(FeatureBranchPrefix, DevelopBranch);
#pragma warning restore S2583 // Conditionally executed code should be reachable

                Information("Good bye !");
            }
            else
            {
                FinishFeature();
            }
        });

    /// <summary>
    /// Asks the user for a branch name
    /// </summary>
    /// <param name="branchNamePrefix">A prefix to preprend in front of the user branch name</param>
    /// <param name="sourceBranch">Branch from which a new branch will be created</param>
    private void AskBranchNameAndSwitchToIt(string branchNamePrefix, string sourceBranch)
    {
        string featureName;
        bool exitCreatingFeature = false;
        do
        {
            featureName = (Name ?? Console.ReadLine() ?? string.Empty).Trim()
                                                            .Trim('/');

            switch (featureName)
            {
                case string name when !string.IsNullOrWhiteSpace(name):
                    {
                        string branchName = $"{branchNamePrefix}/{featureName.Slugify()}";
                        Information($"The branch '{{BranchName}}' will be created.{Environment.NewLine}Confirm ? (Y/N) ", branchName);

                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.Y:
                                Information("Checking out branch '{BranchName}' from '{SourceBranch}'", branchName, sourceBranch);

                                Checkout(branchName, start: sourceBranch);

                                Information("'{BranchName}' created successfully", branchName);
                                exitCreatingFeature = true;
                                break;

                            default:
                                Information($"{Environment.NewLine}Exiting {nameof(Feature)} task.");
                                exitCreatingFeature = true;
                                break;
                        }
                    }
                    break;
                default:
                    Information($"Exiting task.");
                    exitCreatingFeature = true;
                    break;
            }

#pragma warning disable S2583 // Conditionally executed code should be reachable
        } while (string.IsNullOrWhiteSpace(featureName) && !exitCreatingFeature);
    }

    public Target Release => _ => _
        .DependsOn(Changelog)
        .Description($"Starts a new {ReleaseBranchPrefix}/{{version}} from {DevelopBranch}")
        .Requires(() => !GitRepository.IsOnReleaseBranch() || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            if (!GitRepository.IsOnReleaseBranch())
            {
                Checkout($"{ReleaseBranchPrefix}/{GitVersion.MajorMinorPatch}", start: DevelopBranch);
            }
            else
            {
                FinishReleaseOrHotfix();
            }
        });

    public Target Hotfix => _ => _
        .DependsOn(Changelog)
        .Description($"Starts a new hotfix branch '{HotfixBranchPrefix}/*' from {MainBranchName}")
        .Requires(() => !GitRepository.IsOnHotfixBranch() || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            (GitVersion mainBranchVersion, IReadOnlyCollection<Output> _) = GitVersion(s => s
                .SetFramework("net5.0")
                .SetUrl(RootDirectory)
                .SetBranch(MainBranchName)
                .EnableNoFetch()
                .DisableProcessLogOutput());

            if (!GitRepository.IsOnHotfixBranch())
            {
                Checkout($"{HotfixBranchPrefix}/{mainBranchVersion.Major}.{mainBranchVersion.Minor}.{mainBranchVersion.Patch + 1}", start: MainBranchName);
            }
            else
            {
                FinishReleaseOrHotfix();
            }
        });

    public Target Coldfix => _ => _
        .Description($"Starts a new coldfix development by creating the associated '{ColdfixBranchPrefix}/{{name}}' from {DevelopBranch}")
        .Requires(() => IsLocalBuild)
        .Requires(() => !GitRepository.Branch.Like($"{ColdfixBranchPrefix}/*", true) || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            if (!GitRepository.Branch.Like($"{ColdfixBranchPrefix}/*"))
            {
                Information("Enter the name of the coldfix. It will be used as the name of the coldfix/branch (leave empty to exit) :");
                AskBranchNameAndSwitchToIt(ColdfixBranchPrefix, DevelopBranch);
#pragma warning restore S2583 // Conditionally executed code should be reachable
                Information($"{EnvironmentInfo.NewLine}Good bye !");
            }
            else
            {
                FinishColdfix();
            }
        });

    /// <summary>
    /// Merge a coldfix/* branch back to the develop branch
    /// </summary>
    private void FinishColdfix() => FinishFeature();

    private void Checkout(string branch, string start)
    {
        bool hasCleanWorkingCopy = GitHasCleanWorkingCopy();

        if (!hasCleanWorkingCopy && AutoStash)
        {
            Git("stash");
        }
        Git($"checkout -b {branch} {start}");

        if (!hasCleanWorkingCopy && AutoStash)
        {
            Git("stash apply");
        }
    }

    private string MajorMinorPatchVersion => GitVersion.MajorMinorPatch;

    private void FinishReleaseOrHotfix()
    {
        Git($"checkout {MainBranchName}");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");
        Git($"tag {MajorMinorPatchVersion}");

        Git($"checkout {DevelopBranch}");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");

        Git($"branch -D {GitRepository.Branch}");

        Git($"push origin --follow-tags {MainBranchName} {DevelopBranch} {MajorMinorPatchVersion}");
    }

    private void FinishFeature()
    {
        Git($"rebase {DevelopBranch}");
        Git($"checkout {DevelopBranch}");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");

        Git($"branch -D {GitRepository.Branch}");
        Git($"push origin {DevelopBranch}");
    }


    #endregion

    [Parameter("API key used to publish artifacts to Nuget.org")]
    [Secret]
    public readonly string NugetApiKey;

    [Parameter(@"URI where packages should be published (default : ""https://api.nuget.org/v3/index.json""")]
    public string NugetPackageSource => "https://api.nuget.org/v3/index.json";

    public string GitHubPackageSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";

    public bool IsOnGithub => GitHubActions is not null;

    public Target Publish => _ => _
        .Description($"Published packages (*.nupkg and *.snupkg) to the destination server set with {nameof(NugetPackageSource)} settings ")
        .DependsOn(Tests, Pack)
        .Triggers(AddGithubRelease)
        .Consumes(Pack, ArtifactsDirectory / "*.nupkg", ArtifactsDirectory / "*.snupkg")
        .Requires(() => !(NugetApiKey.IsNullOrEmpty() || GitHubToken.IsNullOrEmpty()))
        .Requires(() => GitHasCleanWorkingCopy())
        .Requires(() => GitRepository.IsOnMainBranch()
                        || GitRepository.IsOnReleaseBranch()
                        || GitRepository.IsOnDevelopBranch())
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            void PushPackages(IReadOnlyCollection<AbsolutePath> nupkgs)
            {
                Information($"Publishing {{PackageCount}} package{(nupkgs.Count > 1 ? "s" : string.Empty)}", nupkgs.Count);
                Information(string.Join(EnvironmentInfo.NewLine, nupkgs));

                DotNetNuGetPush(s => s.SetApiKey(NugetApiKey)
                    .SetSource(NugetPackageSource)
                    .EnableSkipDuplicate()
                    .EnableNoSymbols()
                    .CombineWith(nupkgs, (_, nupkg) => _
                                .SetTargetPath(nupkg)),
                    degreeOfParallelism: 4,
                    completeOnFailure: true);

                DotNetNuGetPush(s => s.SetApiKey(GitHubToken)
                    .SetSource(GitHubPackageSource)
                    .EnableSkipDuplicate()
                    .EnableNoSymbols()
                    .CombineWith(nupkgs, (_, nupkg) => _
                                .SetTargetPath(nupkg)),
                    degreeOfParallelism: 4,
                    completeOnFailure: true);
            }

            PushPackages(ArtifactsDirectory.GlobFiles("*.nupkg", "!*TestsHelpers.*nupkg", "!*PerformanceTests.*nupkg"));
            PushPackages(ArtifactsDirectory.GlobFiles("*.snupkg", "!*TestsHelpers.*nupkg", "!*PerformanceTests.*nupkg"));
        });

    public Target AddGithubRelease => _ => _
        .After(Publish)
        .Unlisted()
        .Description("Creates a new GitHub release after *.nupkgs/*.snupkg were successfully published.")
        .OnlyWhenStatic(() => IsServerBuild && GitRepository.IsOnMainBranch())
        .Executes(async () =>
        {
            Information("Creating a new release");
            Octokit.GitHubClient gitHubClient = new(new Octokit.ProductHeaderValue(nameof(Queries)))
            {
                Credentials = new Octokit.Credentials(GitHubToken)
            };

            string repositoryName = GitHubActions.Repository.Replace(GitHubActions.RepositoryOwner + "/", string.Empty);
            IReadOnlyList<Octokit.Release> releases = await gitHubClient.Repository.Release.GetAll(GitHubActions.RepositoryOwner, repositoryName)
                                                                                           .ConfigureAwait(false);

            if (!releases.AtLeastOnce(release => release.Name == MajorMinorPatchVersion))
            {
                Octokit.NewRelease newRelease = new(MajorMinorPatchVersion)
                {
                    TargetCommitish = GitRepository.Commit,
                    Body = string.Concat("- ", ExtractChangelogSectionNotes(ChangeLogFile, MajorMinorPatchVersion).Select(line => $"{line}\n")),
                    Name = MajorMinorPatchVersion,
                };

                Octokit.Release release = await gitHubClient.Repository.Release.Create(GitHubActions.RepositoryOwner, repositoryName, newRelease)
                                                                               .ConfigureAwait(false);

                Information("Github release {TagName} created successfully", release.TagName);
            }
            else
            {
                Information("Release '{Version}' already exists - skipping ", MajorMinorPatchVersion);
            }
        });

    public Target Benchmarks => _ => _
        .Description("Run all performance tests.")
        .DependsOn(Compile)
        .Produces(BenchmarkDirectory / "*")
        .OnlyWhenStatic(() => IsLocalBuild)
        .Executes(() =>
        {
            IEnumerable<Project> benchmarkProjects = Solution.GetProjects("*.PerformanceTests");

            benchmarkProjects.ForEach(action: csproj =>
            {
                DotNetRun(s =>
                {
                    IReadOnlyCollection<string> frameworks = csproj.GetTargetFrameworks();
                    return s.SetConfiguration(Configuration.Release)
                                                        .SetProjectFile(csproj)
                                                        .SetProcessArgumentConfigurator(args => args.Add("-- --filter {0}", "*", customValue: true)
                                                                                                    .Add("--artifacts {0}", BenchmarkDirectory)
                                                                                                    .Add("--join"))
                                                        .CombineWith(frameworks, (setting, framework) => setting.SetFramework(framework));
                });
            });
        });

    ///<inheritdoc/>
    protected override void OnBuildCreated()
    {
        // Small hack until GitVersion 5.8.0 is released (see https://github.com/GitTools/GitVersion/issues/2906#issuecomment-964629657)
        if (IsServerBuild)
        {
            EnvironmentInfo.SetVariable("DOTNET_ROLL_FORWARD", "Major");
        }
    }
}