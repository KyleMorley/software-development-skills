using Modules.Linq.Interfaces;
using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class ConsoleAlertPublisher : ICoreConsolePublisher
{
    /// <inheritdoc />
    public void Publish(List<SensorAlert> alerts)
    {
        foreach (var alert in alerts)
            Console.WriteLine($"ALERT sensor {alert.SensorId}: {alert.TemperatureC} C at {alert.RecordedAt:O}");
    }
}
