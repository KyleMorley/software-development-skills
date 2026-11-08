using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class InvoiceReportService
{
    // One summary per requested account ID, in input order.
    // Duplicates are retained. Missing accounts produce zero-valued summaries.
    // Only paid, non-voided invoices count.
    public List<AccountSummary> BuildSummaries(
        IReadOnlyList<Invoice> invoices,
        IReadOnlyList<int> accountIds)
    {
        var summaries = new List<AccountSummary>();

        foreach (var accountId in accountIds)
        {
            var accountInvoices = invoices
                .Where(invoice => invoice.AccountId == accountId)
                .ToList();

            var eligible = accountInvoices
                .Where(invoice => invoice.IsPaid && !invoice.IsVoided)
                .ToList();

            var total = eligible.Select(invoice => invoice.Amount).ToList().Sum();
            summaries.Add(new AccountSummary(accountId, total, eligible.Count()));
        }

        return summaries;
    }

    // Supported formats are "text" and "csv".
    public string RenderReport(
        IReadOnlyList<AccountSummary> summaries,
        string format)
    {
        if (format == "csv")
        {
            var rows = new List<string> { "AccountId,TotalPaid,PaidInvoiceCount" };
            foreach (var summary in summaries)
                rows.Add($"{summary.AccountId},{summary.TotalPaid.ToString(System.Globalization.CultureInfo.InvariantCulture)},{summary.PaidInvoiceCount}");
            return string.Join(Environment.NewLine, rows);
        }

        if (format == "text")
        {
            var rows = new List<string>();
            foreach (var summary in summaries)
                rows.Add($"Account {summary.AccountId}: {summary.TotalPaid:C} ({summary.PaidInvoiceCount} paid invoices)");
            return string.Join(Environment.NewLine, rows);
        }

        throw new ArgumentException("Unknown report format.", nameof(format));
    }
}
