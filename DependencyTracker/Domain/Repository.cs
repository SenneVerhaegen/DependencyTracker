namespace DependencyTracker.Domain;

public class Repository
{
    public string Name { get; set; }
    public IEnumerable<Solution> Solutions => _solutions.AsReadOnly();
    private readonly List<Solution> _solutions = [];
    public void AddSolution(Solution solution) => _solutions.Add(solution);
}