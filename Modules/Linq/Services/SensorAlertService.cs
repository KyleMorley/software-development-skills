using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class SensorAlertService
{
    // Use each sensor's most recent valid reading (ties: first in input order).
    // Return one alert per sensor only when its temperature is strictly above the limit.
    // Alerts are returned in ascending sensor ID order.
    public List<SensorAlert> FindAlerts(
        IReadOnlyList<SensorReading> readings,
        decimal temperatureLimitC)
    {   
        var alerts = readings
            .Where(reading => reading.IsValid && reading.TemperatureC > temperatureLimitC)
            .GroupBy(reading => reading.SensorId)
            .Select(group => group
                .Max(reading => reading))

            .OrderBy(reading => reading.SensorId)
            .Select(reading => new SensorAlert(
                reading.SensorId,
                reading.Id,
                reading.RecordedAt,
                reading.TemperatureC))
            .ToList();


        return alerts;
    }
}
