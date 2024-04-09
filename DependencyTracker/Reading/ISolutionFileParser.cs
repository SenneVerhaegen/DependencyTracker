namespace DependencyTracker.Reading;

public interface ISolutionFileParser
{
    /// <summary>
    /// Lists the projects inside a solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution. Ends with .sln</param>
    /// <returns>A list of project names.</returns>
    /// <exception cref="InvalidOperationException">If the solution file was not found.</exception>
    IReadOnlyList<string> ListProjects(string solutionPath);
}