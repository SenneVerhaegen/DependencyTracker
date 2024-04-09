namespace DependencyTracker.Domain;

public class Solution
{
    public required string FileName { get; init; }
    public required string Name { get; set; }
    public required string Url { get; init; }
    public IEnumerable<Project> Projects => _projects.AsReadOnly();
    private readonly List<Project> _projects = [];
    public void AddProject(Project project) => _projects.Add(project);
}