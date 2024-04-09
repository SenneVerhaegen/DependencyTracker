namespace DependencyTracker.Reporting;

public class AnalysisReport
{
    private readonly List<NuGetPackage> _packages = [];
    private readonly List<Repository> _repositories = [];

    public IEnumerable<NuGetPackage> Packages => _packages.AsReadOnly();
    public IEnumerable<Repository> Repositories => _repositories.AsReadOnly();

    public void AddNuGetPackage(NuGetPackage reportEntry) => _packages.Add(reportEntry);

    public void AddRepository(Repository repository) => _repositories.Add(repository);
}