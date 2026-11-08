namespace Modules.Linq.Models;

public sealed record WarehouseSummary(
    int WarehouseId,
    decimal TotalWeightKg,
    int ShipmentCount);
