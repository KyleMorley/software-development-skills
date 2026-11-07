namespace Modules.SOLID.Interfaces;

public interface IConsoleOrderNotifier
{
    /// <summary>
    /// Notifies customer that there order was processed
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="orderId"></param>
    public void Notify(string customer, string orderId);
}