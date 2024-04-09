namespace DependencyTracker.Reporting;

public class Repository
{
    private readonly List<Solution> _solutions = [];

    public Repository(string name, AnalysisReport report)
    {
        Name = name;
        report.AddRepository(this);
    }

    public string Name { get; init; }
    public IEnumerable<Solution> Solutions => _solutions.AsReadOnly();

    public void AddSolution(Solution solution) => _solutions.Add(solution);
}