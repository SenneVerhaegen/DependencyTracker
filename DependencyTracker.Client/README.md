# Dependency Tracker

A tool that analyzes NuGet dependencies in your repositories / solutions / projects and outputs a report that shows
which dependencies are not up-to-date.

## Configuration

Configuration values are loaded from:

- environment variables
- user-secrets
- appsettings.json

```json
{
  "AzureDevopsPat": "",
  "GitHubPat": "",
  "TempDir": "",
  "OutputDir": "",
  "Repositories": {
    "AzureDevops": [
      {
        "Organization": "",
        "Project": "",
        "RepositoryId": ""
      }
    ],
    "GitHub": [
      {
        "Owner": "",
        "Repository": ""
      }
    ]
  }
}
```

| Key                                   | Description                                                                            | Required                                             |
|---------------------------------------|----------------------------------------------------------------------------------------|------------------------------------------------------|
| AzureDevopsPat                        | Personal Access Token for azure devops. Necessary if the repository is in Azure Devops | No. Only required if using Azure Devops repositories |
| GitHubPat                             | Personal Access Token for GitHUb. Necessary if the repository is in GitHub             | No. Only required if using GitHub repositories       |
| TempDir                               | Temporary directory to download `.sln` and `.csproj` files to.                         | Yes                                                  |
| OutputDir                             | Output directory to write the html report to if using `HtmlReportService`.             | No. Only required if using `HtmlReportService`       |
| Repositories                          | Configures the required data for fetching your repositories using the platform's API.  | Yes                                                  |
| Repositories:AzureDevops              | Azure Devops repositories                                                              | No                                                   |
| Repositories:AzureDevops:Organization | Name of the organization.                                                              | Yes                                                  |
| Repositories:AzureDevops:Project      | Name of the project.                                                                   | Yes                                                  |
| Repositories:AzureDevops:RepositoryId | Name of the git repository.                                                            | Yes                                                  |
| Repositories:GitHub                   | GitHub repositories                                                                    | No                                                   |
| Repositories:GitHub:Owner             | Owner of the repository.                                                               | Yes                                                  |
| Repositories:AzureDevops:Repository   | Name of the git repository.                                                            | Yes                                                  |

## Reporting

By default, a report of the analysis is created as a html file and written to `OutputDir`.
It is possible to write custom implementations for `IReportService<T>`.
For example: email a report, send a slack notification, ...

## API Permissions

### Azure Devops

The PAT needs the following permissions:

- todo

### GitHub

The PAT needs the following permissions:

- todo
