using Modukes.Linq.Interfaces;
using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class InvoiceReportService(IReadOnlyDictionary<string, IInvoiceSummaryFormattingService> formattingServices)
{
    public List<AccountSummary> BuildSummaries(
        IReadOnlyList<Invoice> invoices,
        IReadOnlyList<int> accountIds)
    {
        var summaries = new List<AccountSummary>();
        var accountIdsSet = new HashSet<int>(accountIds);
        var invoicesLookup = invoices
            .Where(invoice => accountIdsSet.Contains(invoice.AccountId) && invoice.IsPaid && !invoice.IsVoided)
            .ToLookup(Invoice => Invoice.AccountId);

        foreach (var accountId in accountIds)
        {
            var eligible = invoicesLookup[accountId];

            var total = eligible.Sum(invoice => invoice.Amount);
            summaries.Add(new AccountSummary(accountId, total, eligible.Count()));
        }

        return summaries;
    }

    // Supported formats are "text" and "csv".
    public string RenderReport(
        IReadOnlyList<AccountSummary> summaries,
       string format)
    {
        if(!formattingServices.TryGetValue(format, out var formattingService))
        {
            throw new ArgumentException("Unknown report format.", nameof(format));
        }

        return formattingService.FormatInvoiceSummaries(summaries);
    }
}
