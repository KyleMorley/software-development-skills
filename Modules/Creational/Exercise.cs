using System.Net.Http.Headers;
using System.Text.Json;
using Interfaces;

namespace Modules.Creational;

public sealed class Exercise : ILearningModule
{
    public string Name => "Factory Method: Report Exports";
    public InMemoryReportArchive Archive = new() { FailOnId = "R-004" };
    private ReportExportService? Exporter;
    private ProcessingService? ProcessingService;

    public void Run()
    {
        Exporter = new(Archive);
        ProcessingService = new(Exporter);

        var requests = new[]
        {
            new ExportRequest("R-001", "Csv", "Weekly sales", ["North", "South"]),
            new ExportRequest("R-002", "Json", "Open incidents", ["INC-12", "INC-19"]),
            new ExportRequest("R-003", "Csv", "", ["Missing title"]),
            new ExportRequest("R-004", "Json", "Overdue tasks", ["TASK-8"]),
            new ExportRequest("R-005", "Xml", "Unsupported report", ["Example"]),
            new ExportRequest("R-006", "Csv", "Completed work", ["TASK-5"])
        };

        foreach (var result in ProcessingService.RunBatch(requests))
            Console.WriteLine(result);
    }
}

public interface IProcessingService
{
    /// <summary>
    /// Batch process
    /// </summary>
    /// <param name="requests"></param>
    /// <returns></returns>
    public IReadOnlyList<string> RunBatch(
        IEnumerable<ExportRequest> requests);
}

public class ProcessingService(ReportExportService exporter) : IProcessingService
{
    /// <inheritdoc />
    public IReadOnlyList<string> RunBatch(
        IEnumerable<ExportRequest> requests)
    {
        var results = new List<string>();

        foreach (var request in requests)
            results.Add(exporter.Export(request));

        return results;
    }
}

public sealed record ExportRequest(
    string Id,
    string Format,
    string Title,
    IReadOnlyList<string> Entries);

public interface IReportRenderer
{
    string Render(ExportRequest request);
}

public sealed class CsvReportRenderer : IReportRenderer
{
    public string Render(ExportRequest request) =>
        $"CSV::{request.Title}::{string.Join("|", request.Entries)}";
}

public sealed class JsonReportRenderer : IReportRenderer
{
    public string Render(ExportRequest request) =>
        JsonSerializer.Serialize(new { request.Title, request.Entries });
}

public abstract class Creator
{
    public abstract IReportRenderer Create();
}

public class JsonReportPolicy : Creator
{
    public override IReportRenderer Create() => new JsonReportRenderer();
}

public class CsvReportPolicy : Creator
{
    public override IReportRenderer Create() => new CsvReportRenderer();
}


public interface IReportArchive
{
    void Save(string id, string contents);
}

public sealed class InMemoryReportArchive : IReportArchive
{
    private readonly Dictionary<string, string> _documents = [];

    public string? FailOnId { get; init; }
    public IReadOnlyDictionary<string, string> Documents => _documents;

    public void Save(string id, string contents)
    {
        if (id == FailOnId)
            throw new IOException("Report archive is unavailable.");

        _documents.Add(id, contents);
    }
}

public sealed class ReportExportService(IReportArchive archive)
{
    public string Export(ExportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("A report title is required.", nameof(request));

            IReportRenderer renderer;
            if (string.Equals(request.Format, "Csv", StringComparison.OrdinalIgnoreCase))
                renderer = new CsvReportRenderer();
            else if (string.Equals(request.Format, "Json", StringComparison.OrdinalIgnoreCase))
                renderer = new JsonReportRenderer();
            else
                throw new NotSupportedException($"Format '{request.Format}' is not supported.");

            var contents = renderer.Render(request);
            archive.Save(request.Id, contents);

            return $"Saved {request.Id} ({request.Format})";
        }
        catch (Exception exception)
        {
            return $"Failed {request.Id}: {exception.Message}";
        }
    }
}
