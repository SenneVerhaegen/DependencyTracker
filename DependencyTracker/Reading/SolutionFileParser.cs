using System.Text.RegularExpressions;

namespace DependencyTracker.Reading;

internal sealed partial class SolutionFileParser : ISolutionFileParser
{
    /// <summary>
    /// Lists the projects inside a solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution. Ends with .sln</param>
    /// <returns>A list of project names.</returns>
    /// <exception cref="InvalidOperationException">If the solution file was not found.</exception>
    public IReadOnlyList<string> ListProjects(string solutionPath)
    {
        var solutionFileContent = File.ReadAllText(solutionPath);

        var regex = CsprojFileRegex();
        var matches = regex.Matches(solutionFileContent);

        var projects = new List<string>();
        foreach (Match match in matches)
        {
            var projectName = match.Groups[1].Value;
            projects.Add(projectName);
        }

        return projects.AsReadOnly();
    }

    /// <summary>
    /// Regex to extract the project name and path to .csproj file relative to the solution file.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^Project.*""\) = ""([^""]*)"", ""([^""]*)""", RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex CsprojFileRegex();
}