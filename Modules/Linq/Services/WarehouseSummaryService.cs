using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class WarehouseSummaryService
{
    // Return one summary per requested warehouse ID, in input order.
    // Duplicate requested IDs produce duplicate summaries. Warehouses without
    // qualifying shipments receive zero weight and count.
    // Only dispatched, non-cancelled shipments qualify.
    public List<WarehouseSummary> GetWarehouseSummaries(
        IReadOnlyList<Shipment> shipments,
        IReadOnlyList<int> warehouseIds)
    {
        var results = new List<WarehouseSummary>();

        foreach (var warehouseId in warehouseIds)
        {
            var warehouseShipments = shipments
                .Where(shipment => shipment.WarehouseId == warehouseId)
                .ToList();

            var dispatchedShipments = warehouseShipments
                .Where(shipment => shipment.IsDispatched && !shipment.IsCancelled)
                .ToList();

            var totalWeight = dispatchedShipments
                .Select(shipment => shipment.WeightKg)
                .ToList()
                .Sum();

            var count = dispatchedShipments.Count();

            results.Add(new WarehouseSummary(warehouseId, totalWeight, count));
        }

        return results;
    }
}
