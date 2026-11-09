using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class ConsoleAlertPublisher
{
    public void Publish(SensorAlert alert)
    {
        Console.WriteLine(
            $"ALERT sensor {alert.SensorId}: {alert.TemperatureC} C at {alert.RecordedAt:O}");
    }
}
