namespace DependencyTracker.Domain;

public class Project
{
    public required string FileName { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public IEnumerable<NuGetPackage> Packages => _packages.AsReadOnly();
    private readonly List<NuGetPackage> _packages = [];
    public void AddPackage(NuGetPackage package) => _packages.Add(package);
}