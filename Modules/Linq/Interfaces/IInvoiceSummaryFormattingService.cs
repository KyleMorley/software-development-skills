using Modules.Linq.Models;

namespace Modules.Linq.Interfaces;

public interface IInvoiceSummaryFormattingService
{
    /// <summary>
    /// The format of the summarries
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Format the invoice summaries
    /// </summary>
    /// <param name="summaries"></param>
    /// <returns></returns>
    public string FormatInvoiceSummaries(IReadOnlyList<AccountSummary> summaries);
}