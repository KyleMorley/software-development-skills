namespace Modules.SOLID.Interfaces;

public interface IOrderRepository
{
    /// <summary>
    /// Order
    /// </summary>
    public IReadOnlyList<string> Records { get; }

    /// <summary>
    /// Saves an order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="totalCost"></param>
    public void SaveOrder(string orderId, decimal totalCost);
}