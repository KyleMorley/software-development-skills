namespace Modules.Linq.Models;

public sealed record SensorReading(
    int Id,
    int SensorId,
    DateTimeOffset RecordedAt,
    decimal TemperatureC,
    bool IsValid);
