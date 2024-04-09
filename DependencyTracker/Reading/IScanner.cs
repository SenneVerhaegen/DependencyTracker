using DependencyTracker.Configuration;
using DependencyTracker.Domain;

namespace DependencyTracker.Reading;

public interface IScanner
{
    Task<IEnumerable<Repository>> ScanRepositories(RepositoriesConfiguration configuration);
}