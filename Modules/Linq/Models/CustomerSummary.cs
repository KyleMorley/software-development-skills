namespace Modules.Linq.Models;

public sealed record CustomerSummary(
    int CustomerId,
    decimal TotalSpent,
    int OrderCount);
