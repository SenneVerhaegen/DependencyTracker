using System.Net.Http.Json;
using DependencyTracker.Configuration;
using DependencyTracker.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyTracker.Reading.GitHub;

internal sealed class GitHubReader : IReader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubReader> _logger;
    private readonly IProjectFileParser _projectFileParser;

    public ISolutionFileParser SolutionFileParser { get; }
    public string TempDir { get; }
    public Type RepositoryType { get; } = typeof(GitHubRepository);

    public GitHubReader(HttpClient http, ILogger<GitHubReader> logger, IConfiguration configuration,
        ISolutionFileParser solutionFileParser, IProjectFileParser projectFileParser)
    {
        _httpClient = http;
        _logger = logger;
        SolutionFileParser = solutionFileParser;
        _projectFileParser = projectFileParser;

        TempDir = configuration.GetValue<string>("TempDir") ??
                  throw new Exception("Value 'TempDir' not found in configuration");

        var pat = configuration.GetValue<string>("GitHubPat") ??
                  throw new Exception("Value 'GithubPat' not found in configuration");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pat);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
    }

    public async Task<IReadOnlyList<Solution>> FetchSolutionFiles(RepositoryConfiguration repoConfig)
    {
        var gitHubRepo = (GitHubRepository)repoConfig;
        
        var apiUrl =
            $"https://api.github.com/repos/{gitHubRepo.Owner}/{gitHubRepo.Repository}/git/trees/main?recursive=1";

        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GitHubResponse.GitHubTree>();

        if (result == null) throw new Exception("Response is null or could not deserialize");

        var solutions = new List<Solution>();
        var downloadTasks = new List<Task>();

        foreach (var item in result.Tree)
        {
            var fileName = Path.GetFileName(item.Path);

            // Check if the item is a solution file (.sln)
            if (!fileName.EndsWith(".sln")) continue;

            var solution = new Solution
            {
                FileName = fileName,
                Name = fileName[..^Constants.SolutionExtensionLength],
                Url = item.Url
            };

            solutions.Add(solution);
            downloadTasks.Add(
                DownloadFile(gitHubRepo, item.Sha, fileName)
            );
        }

        await Task.WhenAll(downloadTasks);

        return solutions;
    }

    public async Task<IReadOnlyList<Project>> FetchProjectFiles(RepositoryConfiguration repoConfig)
    {
        var gitHubRepo = (GitHubRepository)repoConfig;
        
        var apiUrl =
            $"https://api.github.com/repos/{gitHubRepo.Owner}/{gitHubRepo.Repository}/git/trees/main?recursive=1";

        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GitHubResponse.GitHubTree>();

        if (result == null) throw new Exception("Response is null or could not deserialize");

        var projects = new List<Project>();
        var downloadTasks = new List<Task>();

        foreach (var item in result.Tree)
        {
            var fileName = Path.GetFileName(item.Path);

            // Check if the item is a project file (.csproj)
            if (!fileName.EndsWith(".csproj")) continue;

            var p = new Project
            {
                FileName = fileName,
                Name = fileName[..^Constants.CsprojExtensionLength],
                Url = item.Url
            };

            projects.Add(p);
            downloadTasks.Add(
                DownloadFile(gitHubRepo, item.Sha, fileName)
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

    private async Task DownloadFile(GitHubRepository repo, string fileSha, string fileName)
    {
        _logger.LogDebug("Downloading file {@Url}", fileName);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://api.github.com/repos/{repo.Owner}/{repo.Repository}/git/blobs/{fileSha}")
        };

        // Get the blobs as raw content instead of base64
        request.Headers.Add("Accept", "application/vnd.github.raw+json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string savePath = Path.Combine(TempDir, fileName);

        using Stream contentStream = await response.Content.ReadAsStreamAsync();
        using FileStream fileStream = File.Create(savePath);
        await contentStream.CopyToAsync(fileStream);

        _logger.LogDebug("File downloaded: {@FileName}", fileName);
    }
}