using System.Xml;
using DependencyTracker.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyTracker.Reading;

internal sealed class ProjectFileParser : IProjectFileParser
{
    private readonly ILogger<ProjectFileParser> _logger;
    private readonly string _tempDir;

    public ProjectFileParser(ILogger<ProjectFileParser> logger, IConfiguration configuration)
    {
        _logger = logger;
        _tempDir = configuration.GetValue<string>("TempDir") ??
                   throw new Exception("Value 'TempDir' not found in configuration");
    }

    public Task AddPackagesToProject(Project project)
    {
        var path = Path.Combine(_tempDir, project.FileName);
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var reader = XmlReader.Create(fileStream);

        // Move to the first element
        reader.MoveToContent();

        while (reader.Read())
        {
            if (reader is not { NodeType: XmlNodeType.Element, Name: "PackageReference" }) continue;

            var packageId = reader.GetAttribute("Include");
            var packageVersion = reader.GetAttribute("Version");

            if (string.IsNullOrEmpty(packageId))
            {
                _logger.LogInformation(
                    "No attribute 'Include' present on element PackageReference. Skipping package...");
                continue;
            }

            if (string.IsNullOrEmpty(packageVersion))
            {
                _logger.LogInformation(
                    "No attribute 'Version' present on element PackageReference. Skipping package...");
                continue;
            }

            project.AddPackage(new NuGetPackage(packageId, packageVersion));
        }

        return Task.CompletedTask;
    }
}