namespace DependencyTracker.Reporting;

public interface IReportService<T>
{
    Task<T> Report(AnalysisReport report);
}