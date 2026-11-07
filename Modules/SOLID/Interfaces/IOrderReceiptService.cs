using Modules.SOLID.Models;

namespace Modules.SOLID.Interfaces;

public interface IOrderReceiptService
{
    /// <summary>
    /// Formats a receipt from order and cost information
    /// </summary>
    /// <param name="order"></param>
    /// <param name="costs"></param>
    /// <returns></returns>
    public string GenerateReceipt(Order order, OrderCosts costs);
}