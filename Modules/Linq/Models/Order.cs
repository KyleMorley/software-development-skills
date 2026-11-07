namespace Modules.Linq.Models;

public sealed record Order(
    int Id,
    int CustomerId,
    decimal Amount,
    bool IsPaid,
    bool IsCancelled);
