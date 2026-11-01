using Interfaces;

namespace Modules.SOLID.SRP;

public class Exercise : ILearningModule
{
    public string Name => "Single Responsibility Principle";

    public void Run()
    {
        var orders = new[]
        {
            new Order("A-104", "Maya", [new OrderLine("Notebook", 2, 12.50m), new OrderLine("Pen", 3, 2m)], true),
            new Order("B-205", "Leo", [new OrderLine("Folder", 1, 8m)], false)
        };

        var processor = new OrderProcessor();
        foreach (var order in orders)
        {
            Console.WriteLine(processor.Process(order));
            Console.WriteLine();
        }

        foreach (var entry in processor.AuditEntries)
            Console.WriteLine(entry);
    }
}

public record OrderLine(string Description, int Quantity, decimal UnitPrice);
public record Order(string Id, string Customer, IReadOnlyList<OrderLine> Lines, bool PreferredCustomer);

// Order processor currently has multiple reasons to chane
// 1. Anytime a new property is required
// 2. Anytime the processing changes

public class OrderProcessor
{
    private readonly List<string> _auditEntries = [];
    public IReadOnlyList<string> AuditEntries => _auditEntries;

    public string Process(Order order)
    {
        decimal subtotal = 0m;
        foreach (var line in order.Lines)
        {
            if (line.Quantity <= 0 || line.UnitPrice < 0)
                throw new ArgumentException("Invalid order line.");

            subtotal += line.Quantity * line.UnitPrice;
        }

        decimal discount = order.PreferredCustomer && subtotal >= 25m
            ? Math.Round(subtotal * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;
        decimal total = subtotal - discount;

        var receipt = new List<string>
        {
            $"Order {order.Id} for {order.Customer}",
            "------------------------------"
        };
        foreach (var line in order.Lines)
            receipt.Add($"{line.Description}: {line.Quantity} x {line.UnitPrice:0.00}");

        receipt.Add($"Subtotal: {subtotal:0.00}");
        receipt.Add($"Discount: {discount:0.00}");
        receipt.Add($"Total: {total:0.00}");

        _auditEntries.Add($"PROCESSED|{order.Id}|{total:0.00}");
        return string.Join(Environment.NewLine, receipt);
    }
}

public interface IProcessService
{
    /// <summary>
    /// Processes and order
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public string Process(Order order);
}

public class ProcessService : IProcessService
{
    
}
