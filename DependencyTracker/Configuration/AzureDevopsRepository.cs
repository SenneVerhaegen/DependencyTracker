namespace DependencyTracker.Configuration;

public sealed class AzureDevopsRepository : RepositoryConfiguration
{
    public required string Organization { get; set; }
    public required string Project { get; set; }
    public required string RepositoryId { get; set; }
}