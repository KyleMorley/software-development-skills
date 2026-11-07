using Modules.SOLID.Interfaces;

namespace Modules.SOLID.Services;

public class ConsoleOrderNotifierService : IConsoleOrderNotifier
{
    /// <inheritdoc />
    public void Notify(string customer, string orderId)
    {
        Console.WriteLine($"NOTIFY|{customer}|{orderId}");
    }
}