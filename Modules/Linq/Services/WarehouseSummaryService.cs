using Modules.Linq.Models;
using Modules.SOLID.Services;

namespace Modules.Linq.Services;

public sealed class WarehouseSummaryService
{
    public List<WarehouseSummary> GetWarehouseSummaries(
        IReadOnlyList<Shipment> shipments,
        IReadOnlyList<int> warehouseIds)
    {
        var results = new List<WarehouseSummary>();
        HashSet<int> hashedWarehouseIds = [.. warehouseIds];
        var shipmentsLookup = shipments.Where(shipment => hashedWarehouseIds.Contains(shipment.WarehouseId) && shipment.IsDispatched && !shipment.IsCancelled).ToLookup(shipment => shipment.WarehouseId);

        foreach (var warehouseId in warehouseIds)
        {
            var dispatchedShipments = shipmentsLookup[warehouseId];
            var totalWeight = dispatchedShipments.Sum(shipment => shipment.WeightKg);
            var count = dispatchedShipments.Count();

            results.Add(new WarehouseSummary(warehouseId, totalWeight, count));
        }

        return results;
    }
}
