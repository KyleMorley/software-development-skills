using System.Diagnostics;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Development.Tests;

public class WarehouseSummaryServiceTests
{
    [Fact]
    public void GetWarehouseSummaries_Performance_Baseline()
    {
        // Arrange
        var shipments = CreateShipments(200_000);
        var warehouseIds = Enumerable.Range(1, 20_000).ToArray();
        var service = new WarehouseSummaryService();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var results = service.GetWarehouseSummaries(shipments, warehouseIds);
        stopwatch.Stop();
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Seconds} s");
        
        // Assert
        Assert.Equal(20000, results.Count);
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
