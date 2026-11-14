namespace Modules.Linq.Models;

public sealed record SensorAlert(
    int SensorId,
    int ReadingId,
    DateTimeOffset RecordedAt,
    decimal TemperatureC);
