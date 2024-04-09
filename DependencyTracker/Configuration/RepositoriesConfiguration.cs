namespace DependencyTracker.Configuration;

public class RepositoriesConfiguration
{
    public IEnumerable<AzureDevopsRepository>? AzureDevops { get; set; }

    public IEnumerable<GitHubRepository>? GitHub { get; set; }

    public IEnumerable<RepositoryConfiguration> Repositories
    {
        get
        {
            var repositories = new List<RepositoryConfiguration>();

            if (AzureDevops is not null) repositories.AddRange(AzureDevops);

            if (GitHub is not null) repositories.AddRange(GitHub);

            return repositories;
        }
    }
}