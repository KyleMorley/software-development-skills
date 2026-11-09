using System.Diagnostics;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Development.Tests;

public class SensorAlertServiceTests
{
    [Fact]
    public void FindAndPublishAlerts_Perfomance_Baseline()
    {
        // Arrange
        var readings = CreateReadings(20_000_000);
        var service = new SensorAlertService();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = service.FindAlerts(readings, 85m);
        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(104, result.Count);
    }
    
    [Fact]
    public void FindAndPublishAlerts_UsesLatestValidReadingPerSensor()
    {
        var time = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        SensorReading[] readings =
        [
            new(1, 2, time.AddMinutes(1), 95m, true),
            new(2, 2, time.AddMinutes(2), 70m, true),
            new(3, 1, time.AddMinutes(1), 91m, true),
            new(4, 1, time.AddMinutes(2), 94m, false)
        ];

        var alerts = new SensorAlertService().FindAlerts(readings, 85m);

        var alert = Assert.Single(alerts);
        Assert.Equal(1, alert.SensorId);
        Assert.Equal(3, alert.ReadingId);
        Assert.Equal(91m, alert.TemperatureC);
    }

    [Fact]
    public void FindAndPublishAlerts_OrdersResultsAndUsesStrictLimit()
    {
        var time = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        SensorReading[] readings =
        [
            new(1, 7, time, 86m, true),
            new(2, 2, time, 90m, true),
            new(3, 3, time, 85m, true),
            new(4, 4, time, 99m, false)
        ];

        var alerts = new SensorAlertService().FindAlerts(readings, 85m);

        Assert.Equal(new[] { 2, 7 }, alerts.Select(alert => alert.SensorId));
    }

    private static List<SensorReading> CreateReadings(int count)
    {
        var random = new Random(42);
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        return Enumerable.Range(1, count)
            .Select(id => new SensorReading(
                id,
                random.Next(1, 401),
                start.AddMinutes(random.Next(0, 300_000)),
                random.Next(-100, 1_200) / 10m,
                random.Next(0, 10) != 0))
            .ToList();
    }
}
