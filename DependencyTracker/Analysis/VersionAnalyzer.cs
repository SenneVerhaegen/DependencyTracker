using DependencyTracker.Extensions;
using DependencyTracker.Reporting;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DependencyTracker.Analysis;

internal sealed class VersionAnalyzer : IVersionAnalyzer
{
    public async Task<AnalysisReport> Analyze(IEnumerable<Domain.Repository> repositories)
    {
        var report = new AnalysisReport();

        foreach (var repository in repositories)
        {
            var r = new Reporting.Repository(repository.Name, report);

            foreach (var solution in repository.Solutions)
            {
                var s = new Reporting.Solution(solution.Name, r);

                foreach (var project in solution.Projects)
                {
                    var p = new Reporting.Project(project.Name, s);

                    foreach (var package in project.Packages)
                    {
                        await AddPackageAnalysis(package, p, report);
                    }
                }
            }
        }

        return report;
    }

    private async Task AddPackageAnalysis(Domain.NuGetPackage package, Project p, AnalysisReport report)
    {
        var latestVersion = await GetLatestVersion(package.Id);

        if (latestVersion is null) throw new Exception($"Unable to get version for {package.Id}");

        if (package.IsSemanticVersion)
        {
            var latestVersionString = latestVersion.MajorMinorPatch();

            var pkg = new Reporting.NuGetPackage(package.Id, package.Version, latestVersionString, p);

            report.AddNuGetPackage(pkg);
        }
        else
        {
            throw new NotImplementedException(
                "Analysis for non-semantic versions are not (yet) implemented");
        }
    }

    private async Task<NuGetVersion?> GetLatestVersion(string packageId)
    {
        var sourceRepository =
            NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();

        var packages =
            await packageMetadataResource.GetMetadataAsync(packageId, false, false,
                new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);

        // Filter to non-prerelease versions and get the latest version
        var latestVersion = packages
            .Select(p => p.Identity.Version)
            .OrderByDescending(v => v)
            .FirstOrDefault();

        return latestVersion;
    }
}