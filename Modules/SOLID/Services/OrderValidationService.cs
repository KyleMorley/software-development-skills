using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;

namespace Modules.SOLID.Services;

public class OrderValidationService : IOrderValidationService
{
    /// <inheritdoc />
    public void ValidateOrder(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.Id) ||
            string.IsNullOrWhiteSpace(order.Customer) ||
            order.Quantity <= 0 ||
            (order.ServiceLevel != "Standard" && order.ServiceLevel != "Express"))
        {
            throw new ArgumentException("Invalid order.");
        }
    }
}