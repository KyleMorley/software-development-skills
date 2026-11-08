using Modukes.Linq.Interfaces;
using Modules.Linq.Models;

namespace Modules.Linq.Services;

public class CsvFormattingService : IInvoiceSummaryFormattingService
{
    /// <inheritdoc />
    public string Format => "csv";

    /// <inheritdoc />
    public string FormatInvoiceSummaries(IReadOnlyList<AccountSummary> summaries)
    {
        var rows = new List<string> { "AccountId,TotalPaid,PaidInvoiceCount" };

        foreach (var summary in summaries)
        {
            rows.Add($"{summary.AccountId},{summary.TotalPaid.ToString(System.Globalization.CultureInfo.InvariantCulture)},{summary.PaidInvoiceCount}");
        }
                
        return string.Join(Environment.NewLine, rows);
    }
}