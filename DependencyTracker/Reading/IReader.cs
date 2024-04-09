using DependencyTracker.Configuration;
using DependencyTracker.Domain;

namespace DependencyTracker.Reading;

public interface IReader
{
    ISolutionFileParser SolutionFileParser { get; }

    /// <summary>
    /// The type of repository for which instances of this reader read.
    /// </summary>
    Type RepositoryType { get; }

    /// <summary>
    /// Temporary directory to download .sln and .csproj files to.
    /// </summary>
    string TempDir { get; }

    async Task<Repository> FetchRepository(RepositoryConfiguration repoConfig)
    {
        var repository = new Repository();

        var solutions = await FetchSolutionFiles(repoConfig);
        var projects = await FetchProjectFiles(repoConfig);

        foreach (var solution in solutions)
        {
            repository.AddSolution(solution);

            var projectNames = SolutionFileParser.ListProjects(Path.Combine(TempDir, solution.FileName));

            foreach (var project in projects.IntersectBy(projectNames, p => p.Name))
            {
                solution.AddProject(project);
            }
        }

        return repository;
    }

    Task<IReadOnlyList<Solution>> FetchSolutionFiles(RepositoryConfiguration repoConfig);
    Task<IReadOnlyList<Project>> FetchProjectFiles(RepositoryConfiguration repoConfig);
}