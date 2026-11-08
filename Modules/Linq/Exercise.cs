using Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ Query Optimisation: Warehouse Shipments";

    private readonly WarehouseSummaryService warehouseSummaryService = new();

    public void Run()
    {
        var shipments = CreateShipments(20_000);
        var warehouseIds = Enumerable.Range(1, 1_000).ToArray();

        var results = warehouseSummaryService.GetWarehouseSummaries(shipments, warehouseIds);
        Console.WriteLine($"Summaries: {results.Count}");
        foreach (var summary in results.Take(5))
            Console.WriteLine($"{summary.WarehouseId}: {summary.TotalWeightKg} kg ({summary.ShipmentCount} shipments)");
    }

    private static List<Shipment> CreateShipments(int count)
    {
        var random = new Random(42);
        return Enumerable.Range(1, count)
            .Select(id => new Shipment(
                id,
                random.Next(1, 1_201),
                random.Next(100, 20_000) / 100m,
                random.Next(0, 5) != 0,
                random.Next(0, 20) == 0))
            .ToList();
    }
}
