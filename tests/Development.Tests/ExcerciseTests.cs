using System.Text.Json;
using Modules.Creational;

namespace Development.Tests;

public class ReportExportTests
{
    private readonly Exercise ExrciseObject = new();
    private ReportExportService? Exporter;
    private ProcessingService? ProcessingService;

    [Fact]
    public void RunBatch_RendersAndArchivesBothFormats()
    {
        var archive = new InMemoryReportArchive();
        Exporter = new(archive);
        ProcessingService = new(Exporter);
        
        var requests = new[]
        {
            new ExportRequest("A-1", "Csv", "Sales", ["North", "South"]),
            new ExportRequest("A-2", "Json", "Incidents", ["INC-1"])
        };

        var results = ProcessingService.RunBatch(requests);

        Assert.Equal(["Saved A-1 (Csv)", "Saved A-2 (Json)"], results);
        Assert.Equal("CSV::Sales::North|South", archive.Documents["A-1"]);

        using var json = JsonDocument.Parse(archive.Documents["A-2"]);
        Assert.Equal("Incidents", json.RootElement.GetProperty("Title").GetString());
        Assert.Equal("INC-1", json.RootElement.GetProperty("Entries")[0].GetString());
    }

    [Fact]
    public void RunBatch_ReportsInvalidInputsAndContinuesProcessing()
    {
        var archive = new InMemoryReportArchive();
        Exporter = new(archive);
        ProcessingService = new(Exporter);

        var requests = new[]
        {
            new ExportRequest("A-1", "Csv", "", ["Invalid"]),
            new ExportRequest("A-2", "Xml", "Unsupported", ["Invalid"]),
            new ExportRequest("A-3", "Csv", "Valid", ["Done"])
        };

        var results = ProcessingService.RunBatch(requests);

        Assert.StartsWith("Failed A-1: A report title is required.", results[0]);
        Assert.Equal("Failed A-2: Format 'Xml' is not supported.", results[1]);
        Assert.Equal("Saved A-3 (Csv)", results[2]);
        Assert.Single(archive.Documents);
        Assert.True(archive.Documents.ContainsKey("A-3"));
    }

    [Fact]
    public void RunBatch_ReportsArchiveFailureAndContinuesProcessing()
    {
        var archive = new InMemoryReportArchive { FailOnId = "A-2" };
        Exporter = new(archive);
        ProcessingService = new(Exporter);

        var requests = new[]
        {
            new ExportRequest("A-1", "Csv", "Before", ["One"]),
            new ExportRequest("A-2", "Json", "Unavailable", ["Two"]),
            new ExportRequest("A-3", "Csv", "After", ["Three"])
        };

        var results = ProcessingService.RunBatch(requests);

        Assert.Equal("Saved A-1 (Csv)", results[0]);
        Assert.Equal("Failed A-2: Report archive is unavailable.", results[1]);
        Assert.Equal("Saved A-3 (Csv)", results[2]);
        Assert.Equal(2, archive.Documents.Count);
        Assert.False(archive.Documents.ContainsKey("A-2"));
    }
}
