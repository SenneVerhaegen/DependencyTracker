using System.Net.Http.Json;
using DependencyTracker.Configuration;
using DependencyTracker.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyTracker.Reading.AzureDevops;

internal sealed class AzureDevopsReader : IReader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AzureDevopsReader> _logger;
    private readonly IProjectFileParser _projectFileParser;

    private const string ApiVersion = "7.0";

    public ISolutionFileParser SolutionFileParser { get; }
    public string TempDir { get; }
    public Type RepositoryType { get; } = typeof(AzureDevopsRepository);


    public AzureDevopsReader(HttpClient http, ILogger<AzureDevopsReader> logger, IConfiguration configuration,
        ISolutionFileParser solutionFileParser, IProjectFileParser projectFileParser)
    {
        _httpClient = http;
        _logger = logger;
        SolutionFileParser = solutionFileParser;
        _projectFileParser = projectFileParser;

        TempDir = configuration.GetValue<string>("TempDir") ??
                  throw new Exception("Value 'TempDir' not found in configuration");

        var pat = configuration.GetValue<string>("AzureDevopsPat") ??
                  throw new Exception("Value 'AzureDevopsPat' not found in configuration");

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}")));
    }

    /// <summary>
    /// Fetches the solution files from Azure Devops and downloads them in a temporary directory.
    /// </summary>
    /// <param name="repoConfig"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Solution>> FetchSolutionFiles(RepositoryConfiguration repoConfig)
    {
        var azureDevopsRepo = (AzureDevopsRepository)repoConfig;
        var apiUrl =
            $"https://dev.azure.com/{azureDevopsRepo.Organization}/{azureDevopsRepo.Project}/_apis/git/repositories/{azureDevopsRepo.RepositoryId}/items?scopePath=&recursionLevel=Full&api-version={ApiVersion}";

        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetItemResponse>();

        if (result == null) throw new Exception("Response is null or could not deserialize");

        var solutions = new List<Solution>();
        var downloadTasks = new List<Task>();

        foreach (GetItemResponse.Value item in result.Values)
        {
            var itemName = item.Path;

            // Check if the item is a solution file (.sln)
            if (!itemName.EndsWith(".sln")) continue;

            var fileName = Path.GetFileName(new Uri(item.Url).LocalPath);
            var solution = new Solution
            {
                FileName = fileName,
                Name = fileName[..^Constants.SolutionExtensionLength],
                Url = item.Url
            };

            solutions.Add(solution);
            downloadTasks.Add(
                DownloadFile(solution.Url)
            );
        }

        await Task.WhenAll(downloadTasks);

        return solutions;
    }

    /// <summary>
    /// Fetches the csproj files from Azure Devops and downloads them in a temporary directory.
    /// </summary>
    /// <param name="repoConfig"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Project>> FetchProjectFiles(RepositoryConfiguration repoConfig)
    {
        var azureDevopsRepo = (AzureDevopsRepository)repoConfig;
        var apiUrl =
            $"https://dev.azure.com/{azureDevopsRepo.Organization}/{azureDevopsRepo.Project}/_apis/git/repositories/{azureDevopsRepo.RepositoryId}/items?scopePath=&recursionLevel=Full&api-version={ApiVersion}";

        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetItemResponse>();

        if (result == null) throw new Exception("Response is null or could not deserialize");

        var projects = new List<Project>();
        var downloadTasks = new List<Task>();

        foreach (GetItemResponse.Value item in result.Values)
        {
            var itemName = item.Path;

            // Check if the item is a project file (.csproj)
            if (!itemName.EndsWith(".csproj")) continue;

            var fileName = Path.GetFileName(new Uri(item.Url).LocalPath);
            var p = new Project
            {
                FileName = fileName,
                Name = fileName[..^Constants.CsprojExtensionLength],
                Url = item.Url
            };

            projects.Add(p);
            downloadTasks.Add(
                DownloadFile(p.Url)
            );
        }

        // First download the .csproj files
        await Task.WhenAll(downloadTasks);

        // Now we can parse the files
        await AddPackagesToProjects(projects);

        return projects;
    }

    private async Task AddPackagesToProjects(IReadOnlyCollection<Project> projects)
    {
        var parseTasks = new List<Task>(projects.Count);

        foreach (var project in projects)
        {
            parseTasks.Add(_projectFileParser.AddPackagesToProject(project));
        }

        await Task.WhenAll(parseTasks);
    }

    private async Task DownloadFile(string fileUrl)
    {
        _logger.LogDebug("Downloading file {@Url}", fileUrl);

        var response = await _httpClient.GetAsync(fileUrl);
        response.EnsureSuccessStatusCode();

        string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        string savePath = Path.Combine(TempDir, fileName);

        using Stream contentStream = await response.Content.ReadAsStreamAsync();
        using FileStream fileStream = File.Create(savePath);
        await contentStream.CopyToAsync(fileStream);

        _logger.LogDebug("File downloaded: {@FileName}", fileName);
    }
}