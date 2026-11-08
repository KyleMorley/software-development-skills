using Interfaces;
using Modukes.Linq.Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ + Design: Invoice Reporting";
    private readonly IReadOnlyDictionary<string, IInvoiceSummaryFormattingService> FormattingServices = 
        new Dictionary<string, IInvoiceSummaryFormattingService>
    {
        ["csv"] = new CsvFormattingService(),
        ["text"] = new TextFormattingService()
    };
    
    public void Run()
    {
        var invoices = CreateInvoices(20_000);
        var accountIds = Enumerable.Range(1, 1_000).ToArray();
        var service = new InvoiceReportService(FormattingServices);

        var summaries = service.BuildSummaries(invoices, accountIds);
        Console.WriteLine(service.RenderReport([.. summaries.Take(5)], "text"));
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
}
