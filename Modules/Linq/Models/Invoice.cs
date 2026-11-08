namespace Modules.Linq.Models;

public sealed record Invoice(
    int Id,
    int AccountId,
    decimal Amount,
    bool IsPaid,
    bool IsVoided);
