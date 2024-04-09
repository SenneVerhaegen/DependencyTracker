namespace DependencyTracker.Reporting;

public class NuGetPackage
{
    public NuGetPackage(string packageId, string installedVersion, string latestVersion, Project project)
    {
        PackageId = packageId;
        InstalledVersion = installedVersion;
        LatestVersion = latestVersion;
        Project = project;
        Project.AddPackage(this);
    }

    public string PackageId { get; }
    public string InstalledVersion { get; }
    public string LatestVersion { get; }
    public bool LatestVersionInstalled => InstalledVersion == LatestVersion;
    public Project Project { get; }
}