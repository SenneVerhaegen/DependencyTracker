using System.Reflection;
using DependencyTracker.Configuration;

namespace DependencyTracker.Reading;

internal class ReaderProvider
{
    private readonly Dictionary<Type, IReader> _readers = new();

    public ReaderProvider(IServiceProvider sp)
    {
        var assembly = Assembly.GetAssembly(typeof(IReader));

        // Get all implementations of IReader
        var readerTypes = assembly!
            .GetTypes()
            .Where(t => typeof(IReader).IsAssignableFrom(t) &&
                        t is { IsInterface: false, IsAbstract: false, IsClass: true });

        foreach (var readerType in readerTypes)
        {
            var readerService = sp.GetService(readerType) as IReader;

            if (readerService != null)
            {
                _readers[readerService.RepositoryType] = readerService;
            }
        }
    }

    /// <summary>
    /// Returns a concrete instance of <see cref="IReader"/> for the specified <see cref="type"/>.
    /// </summary>
    /// <param name="type">Type of <see cref="RepositoryConfiguration"/>.</param>
    /// <returns>A concrete <see cref="IReader"/>.</returns>
    /// <exception cref="Exception">When no instance was found for the given type.</exception>
    internal IReader Provide(Type type)
    {
        var found = _readers.TryGetValue(type, out var reader);

        if (found == false) throw new Exception($"Could not provide type {type.FullName}");

        return reader!;
    }
}