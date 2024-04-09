using System.Text;
using DependencyTracker.Helpers.Types;
using Microsoft.Extensions.Configuration;

namespace DependencyTracker.Reporting;

public class HtmlReportService : IReportService<Unit>
{
    private readonly string _outputDir;

    public HtmlReportService(IConfiguration configuration)
    {
        _outputDir = configuration.GetValue<string>("OutputDir") ??
                     throw new Exception("Key 'OutputDir' was not found in configuration");
    }

    public async Task<Unit> Report(AnalysisReport report)
    {
        var html = await GetReportAsHtml(report);
        var filePath = Path.Combine(_outputDir, "output.html");
        await File.WriteAllTextAsync(filePath, html);
        return Unit.Value;
    }

    private Task<string> GetReportAsHtml(AnalysisReport report)
    {
        var htmlBuilder = new StringBuilder();

        htmlBuilder.Append("<html><head><style>");
        htmlBuilder.Append(".solution { border: 1px solid black; margin-bottom: 10px; }");
        htmlBuilder.Append(".project { border: 1px solid blue; margin-bottom: 5px; }");
        htmlBuilder.Append(".package-table { border: 1px solid green; border-collapse: collapse; }");
        htmlBuilder.Append(".package-table th, .package-table td { border: 1px solid green; padding: 5px; }");
        htmlBuilder.Append("</style></head><body>");

        foreach (var repository in report.Repositories)
        {
            foreach (var solution in repository.Solutions)
            {
                htmlBuilder.Append($"<div class=\"solution\"><h2>{solution.Name}</h2>");
                foreach (var project in solution.Projects)
                {
                    htmlBuilder.Append($"<div class=\"project\"><h3>{project.Name}</h3>");
                    if (project.Packages.Any())
                    {
                        htmlBuilder.Append(
                            "<table class=\"package-table\"><thead><tr><th>Package Name</th><th>Installed Version</th><th>Latest Version</th></tr></thead><tbody>");
                        foreach (var package in project.Packages)
                        {
                            if (package.LatestVersionInstalled)
                                htmlBuilder.Append(
                                    $"<tr><td>{package.PackageId}</td><td>{package.InstalledVersion}</td><td>{package.LatestVersion}</td></tr>");
                            else
                                htmlBuilder.Append(
                                    $"<tr bgcolor=\"red\"><td>{package.PackageId}</td><td>{package.InstalledVersion}</td><td>{package.LatestVersion}</td></tr>");
                        }

                        htmlBuilder.Append("</tbody></table>");
                    }

                    htmlBuilder.Append("</div>");
                }

                htmlBuilder.Append("</div>");
            }
        }

        htmlBuilder.Append("</body></html>");

        return Task.FromResult(htmlBuilder.ToString());
    }
}