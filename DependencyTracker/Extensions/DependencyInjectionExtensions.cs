using DependencyTracker.Analysis;
using DependencyTracker.Configuration;
using DependencyTracker.Helpers.Types;
using DependencyTracker.Reading;
using DependencyTracker.Reading.AzureDevops;
using DependencyTracker.Reading.GitHub;
using DependencyTracker.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyTracker.Extensions;

public static class DependencyInjectionExtensions
{
    public const string AzureDevopsReaderKey = "AzureDevopsReader";
    public const string GitHubReaderKey = "GitHubReader";

    public static IServiceCollection AddDependencyTracker(this IServiceCollection services,
        Action<ReaderOptions> configureReaders)
    {
        services
            .AddSingleton<IVersionAnalyzer, VersionAnalyzer>()
            .AddSingleton<IScanner, Scanner>()
            .AddSingleton<ISolutionFileParser, SolutionFileParser>()
            .AddSingleton<IProjectFileParser, ProjectFileParser>()
            .AddSingleton<IReportService<Unit>, HtmlReportService>()
            .AddSingleton<ReaderProvider>()
            .AddOptions()
            .AddHttpClient();

        var readerOptions = new ReaderOptions(services);
        configureReaders(readerOptions);

        return services;
    }

    public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RepositoriesConfiguration>(
            configuration.GetSection("Repositories")
        );
    }

    public class ReaderOptions
    {
        internal IServiceCollection Services { get; }

        internal ReaderOptions(IServiceCollection services)
        {
            Services = services;
        }
    }

    public static ReaderOptions AddGitHubReader(this ReaderOptions options)
    {
        options.Services.AddSingleton<GitHubReader>();
        return options;
    }

    public static ReaderOptions AddAzureDevopsReader(this ReaderOptions options)
    {
        options.Services.AddSingleton<AzureDevopsReader>();
        return options;
    }
}