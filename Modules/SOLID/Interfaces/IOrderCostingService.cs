using Modules.SOLID.Models;

namespace Modules.SOLID.Interfaces;

public interface IOrderCostingService
{
    /// <summary>
    /// Calculates costs for an order
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public OrderCosts CalculateCosts(Order order);
}
