using Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ + Design: Sensor Monitoring";
    public SensorAlertService SensorAlertService = new();
    public ConsoleAlertPublisher ConsoleAlertPublisher = new();
    
    public void Run()
    {
        var readings = CreateReadings(100_000);

        var alerts = SensorAlertService.FindAlerts(readings, 85m);
        
        ConsoleAlertPublisher.Publish(alerts);

        Console.WriteLine($"Total alerts: {alerts.Count}");
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
