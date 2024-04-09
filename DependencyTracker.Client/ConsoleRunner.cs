using DependencyTracker.Analysis;
using DependencyTracker.Configuration;
using DependencyTracker.Helpers.Types;
using DependencyTracker.Reading;
using DependencyTracker.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DependencyTracker.Client;

public class ConsoleRunner
{
    private readonly IVersionAnalyzer _versionAnalyzer;
    private readonly IReportService<Unit> _reportService;
    private readonly RepositoriesConfiguration _repositoriesConfiguration;
    private readonly IScanner _scanner;
    private readonly ILogger<ConsoleRunner> _logger;
    private readonly string _tempDir;

    public ConsoleRunner(IVersionAnalyzer versionAnalyzer, IReportService<Unit> reportService,
        IOptions<RepositoriesConfiguration> repositoriesConfiguration, IScanner scanner, IConfiguration configuration,
        ILogger<ConsoleRunner> logger)
    {
        _versionAnalyzer = versionAnalyzer;
        _reportService = reportService;
        _scanner = scanner;
        _logger = logger;
        _repositoriesConfiguration = repositoriesConfiguration.Value;
        _tempDir = configuration.GetValue<string>("TempDir") ??
                   throw new Exception("Key 'TempDir' was not found in configuration");
    }

    public async Task Run()
    {
        var repositories = await _scanner.ScanRepositories(_repositoriesConfiguration);
        var report = await _versionAnalyzer.Analyze(repositories);
        _ = await _reportService.Report(report);
        DeleteTempDir();
    }

    private void DeleteTempDir()
    {
        try
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
                Directory.CreateDirectory(_tempDir);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
        }
    }
}