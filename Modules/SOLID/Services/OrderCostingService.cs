using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;

namespace Modules.SOLID.Services;

public class OrderCostingService(IReadOnlyDictionary<string, IServiceLevelPolicy> policies) : IOrderCostingService
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
        decimal fulfilmentFee;

        if (order is PickupOrder)
        {
            fulfilmentFee = 0m;
        }
        else if (order is DeliveryOrder)
        {
            fulfilmentFee = order.DeliveryFee();
        }
        else
        {
            throw new ArgumentException("Unknown order type.");
        }

        var total = subtotal + fulfilmentFee;

        return new(total, subtotal, fulfilmentFee);
    }
}