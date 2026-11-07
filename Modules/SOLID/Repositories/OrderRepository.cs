using Modules.SOLID.Interfaces;

namespace Modules.SOLID.Repositories;

public class OrderRepository : IOrderRepository
{
    /// <summary>
    /// List of records
    /// </summary>
    private readonly List<string> _records = [];

    /// <inheritdoc />
    public IReadOnlyList<string> Records => _records;

    /// <inheritdoc />
    public void SaveOrder(string orderId, decimal totalCost)
    {
        _records.Add($"PROCESSED|{orderId}|{totalCost:0.00}");
    }
}