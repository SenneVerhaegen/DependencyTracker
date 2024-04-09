using NuGet.Versioning;

namespace DependencyTracker.Extensions;

public static class NugetExtensions
{
    /// <summary>
    /// Returns a string with major.minor.patch format.
    /// Drops bulld numbers if present.
    /// </summary>
    /// <param name="nuGetVersion"></param>
    /// <returns>A string in major.minor.patch format</returns>
    public static string MajorMinorPatch(this NuGetVersion nuGetVersion)
    {
        return $"{nuGetVersion.Major}.{nuGetVersion.Minor}.{nuGetVersion.Patch}";
    }
}