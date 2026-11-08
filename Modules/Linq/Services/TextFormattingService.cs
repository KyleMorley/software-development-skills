using Modukes.Linq.Interfaces;
using Modules.Linq.Models;

namespace Modules.Linq.Services;

public class TextFormattingService : IInvoiceSummaryFormattingService
{
    /// <inheritdoc />
    public string Format => "text";

    /// <inheritdoc />
    public string FormatInvoiceSummaries(IReadOnlyList<AccountSummary> summaries)
    {
        var rows = new List<string>();

        foreach (var summary in summaries)
        {
            rows.Add($"Account {summary.AccountId}: {summary.TotalPaid:C} ({summary.PaidInvoiceCount} paid invoices)");
        }
        
        return string.Join(Environment.NewLine, rows);
    }
}