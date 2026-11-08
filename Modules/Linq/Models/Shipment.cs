namespace Modules.Linq.Models;

public sealed record Shipment(
    int Id,
    int WarehouseId,
    decimal WeightKg,
    bool IsDispatched,
    bool IsCancelled);
