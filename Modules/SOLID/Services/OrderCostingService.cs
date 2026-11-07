using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;

namespace Modules.SOLID.Services;

public class OrderCostingService(IReadOnlyDictionary<string, IServiceLevelPolicy> policies, IEnumerable<string> AllowedOrderTypes) : IOrderCostingService
{
    /// <inheritdoc />
    public OrderCosts CalculateCosts(Order order)
    {
        if(!policies.TryGetValue(order.ServiceLevel, out var policy))
        {
            throw new ArgumentException("Unknown service level.");
        }

        var unitPrice = policy.ApplyFee();

        var subtotal = unitPrice * order.Quantity;

        if(!AllowedOrderTypes.Contains(order.GetType().Name))
        {
            throw new ArgumentException("Unknown order type.");
        }

        decimal fulfilmentFee = order.Fee();

        var total = subtotal + fulfilmentFee;

        return new(total, subtotal, fulfilmentFee);
    }
}