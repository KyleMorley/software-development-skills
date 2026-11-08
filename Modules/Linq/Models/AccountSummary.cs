namespace Modules.Linq.Models;

public sealed record AccountSummary(
    int AccountId,
    decimal TotalPaid,
    int PaidInvoiceCount);
