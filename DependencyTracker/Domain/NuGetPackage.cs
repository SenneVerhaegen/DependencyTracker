using NuGet.Versioning;

namespace DependencyTracker.Domain;

public class NuGetPackage
{
    public string Id { get; }
    public string Version { get; }

    public NuGetPackage(string id, string version)
    {
        Id = id;
        Version = version;
    }

    public bool IsSemanticVersion => SemanticVersion.TryParse(Version, out _);

    public override string ToString() => $"Package: Name={Id} Version={Version}";
}