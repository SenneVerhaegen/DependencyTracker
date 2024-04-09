namespace DependencyTracker.Reporting;

public class Project
{
    private readonly List<NuGetPackage> _packages = [];

    public Project(string name, Solution solution)
    {
        Name = name;
        Solution = solution;
        Solution.AddProject(this);
    }

    public string Name { get; }
    public Solution Solution { get; }
    public IEnumerable<NuGetPackage> Packages => _packages.AsReadOnly();
    public void AddPackage(NuGetPackage package) => _packages.Add(package);
}