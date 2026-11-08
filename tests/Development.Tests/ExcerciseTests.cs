using System.Diagnostics;
using Modukes.Linq.Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Development.Testsl;

public class InvoiceReportServiceTests
{
    private readonly IReadOnlyDictionary<string, IInvoiceSummaryFormattingService> FormattingServices = 
        new Dictionary<string, IInvoiceSummaryFormattingService>
    {
        ["csv"] = new CsvFormattingService(),
        ["text"] = new TextFormattingService()
    };
    
    [Fact]
    public void BuildSummaries_Performance_Base()
    {
        // Arrange
        var invoices = CreateInvoices(200_000);
        var accountIds = Enumerable.Range(1, 20_000).ToArray();
        var service = new InvoiceReportService(FormattingServices);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = service.BuildSummaries(invoices, accountIds);
        stopwatch.Stop();
        Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        // Assert
        Assert.Equal(20_000, result.Count);
    }
        private static List<Invoice> CreateInvoices(int count)
    {
        var random = new Random(42);
        return [.. Enumerable.Range(1, count)
            .Select(id => new Invoice(
                id,
                random.Next(1, 1_201),
                random.Next(100, 20_000) / 100m,
                random.Next(0, 5) != 0,
                random.Next(0, 20) == 0))];
    }

    [Fact]
    public void RenderReport_AsText_ReturnsCorrectString()
    {
        // Arrange
        Invoice[] invoices = 
        [
            new Invoice(1, 1, 10m, true, false)
        ];

        var service = new InvoiceReportService(FormattingServices);

        var expectedReport = "Account 1: $10.00 (1 paid invoices)";

        // Act
        var summaries = service.BuildSummaries(invoices, [1]);
        var report = service.RenderReport(summaries, "text");

        // Assert
        Assert.Equal(expectedReport, report);
    }

    [Fact]
    public void RenderReport_AsCsv_ReturnsCorrectString()
    {
        // Arrange
        Invoice[] invoices = 
        [
            new Invoice(1, 1, 10m, true, false)
        ];

        var service = new InvoiceReportService(FormattingServices);

        var expectedReport = "AccountId,TotalPaid,PaidInvoiceCount\n1,10,1";

        // Act
        var summaries = service.BuildSummaries(invoices, [1]);
        var report = service.RenderReport(summaries, "csv");
        Console.WriteLine(report);
        // Assert
        Assert.Equal(expectedReport, report);
    }
}