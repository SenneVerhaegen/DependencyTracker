namespace DependencyTracker.Configuration;

public sealed class GitHubRepository : RepositoryConfiguration
{
    public required string Repository { get; set; }
    public required string Owner { get; set; }
}