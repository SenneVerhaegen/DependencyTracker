using System.Reflection;
using DependencyTracker.Configuration;
using DependencyTracker.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyTracker.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(configurationBuilder =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configurationBuilder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile
                (
                    string.IsNullOrEmpty(env)
                        ? "appsettings.json"
                        : $"appsettings.{env}.json",
                    optional: false
                )
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddEnvironmentVariables();

            // AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)

            // configurationBuilder
        });

        builder.ConfigureServices((context, services) =>
        {
            services
                .AddSingleton<ConsoleRunner>()
                .AddDependencyTracker(configure => configure
                    .AddAzureDevopsReader()
                    .AddGitHubReader()
                )
                .AddRepositories(context.Configuration);
        });

        builder.ConfigureLogging((context, loggingBuilder) => loggingBuilder.AddConsole());

        var app = builder.Build();

        var consoleRunner = app.Services.GetRequiredService<ConsoleRunner>();
        await consoleRunner.Run();
    }
}