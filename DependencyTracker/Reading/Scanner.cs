using DependencyTracker.Configuration;
using DependencyTracker.Domain;

namespace DependencyTracker.Reading;

internal sealed class Scanner : IScanner
{
    private readonly ReaderProvider _readerProvider;

    public Scanner(ReaderProvider readerProvider)
    {
        _readerProvider = readerProvider;
    }

    public async Task<IEnumerable<Repository>> ScanRepositories(RepositoriesConfiguration configuration)
    {
        var repositories = new List<Repository>();

        var fetchTasks = new List<Task<Repository>>();

        fetchTasks.AddRange(configuration.Repositories
            .Select(r =>
            {
                var reader = _readerProvider.Provide(r.GetType());
                return reader.FetchRepository(r);
            }));

        await Task.WhenAll(fetchTasks);

        repositories.AddRange(
            fetchTasks.Select(t => t.Result)
        );

        return repositories.AsReadOnly();
    }
}