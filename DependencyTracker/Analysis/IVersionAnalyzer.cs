using DependencyTracker.Reporting;

namespace DependencyTracker.Analysis;

public interface IVersionAnalyzer
{
    Task<AnalysisReport> Analyze(IEnumerable<Domain.Repository> repositories);
}