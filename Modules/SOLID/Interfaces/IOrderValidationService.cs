using Modules.SOLID.Models;

namespace Modules.SOLID.Interfaces;

public interface IOrderValidationService
{
    /// <summary>
    /// Validates an order
    /// </summary>
    /// <param name="order"></param>
    public void ValidateOrder(Order order);
}