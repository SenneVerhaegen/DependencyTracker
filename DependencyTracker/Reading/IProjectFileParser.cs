using DependencyTracker.Domain;

namespace DependencyTracker.Reading;

public interface IProjectFileParser
{
    Task AddPackagesToProject(Project project);
}