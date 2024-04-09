namespace DependencyTracker.Reporting;

public class Solution
{
    private readonly List<Project> _projects = [];

    public Solution(string name, Repository repository)
    {
        Name = name;
        Repository = repository;
        Repository.AddSolution(this);
    }

    public string Name { get; }
    public Repository Repository { get; }
    public IEnumerable<Project> Projects => _projects.AsReadOnly();

    public void AddProject(Project project) => _projects.Add(project);
}