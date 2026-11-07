using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;

namespace Modules.SOLID.Services;

public class OrderReceiptService : IOrderReceiptService
{
    /// <inheritdoc />
    public string GenerateReceipt(Order order, OrderCosts costs)
    {
        return string.Join(Environment.NewLine,
            $"Order {order.Id} for {order.Customer}",
            $"Service: {order.ServiceLevel}",
            $"Quantity: {order.Quantity}",
            $"Subtotal: {costs.Subtotal:0.00}",
            $"Fulfilment: {costs.FulfilmentFee:0.00}",
            $"Total: {costs.Total:0.00}");
    }
}