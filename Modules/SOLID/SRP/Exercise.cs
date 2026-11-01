using System.Numerics;
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

        var processor = new OrderProcessor(new CalculationService(), new ReceiptService());
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

public record ClaclulatedResults(decimal Subtotal, decimal Discount, decimal Total);

// Order processor currently has multiple reasons to chane
// Suppose the business wants to change the receipt's appearance without changing how discounts are calculated.
// Should the class responsible for calculating discounts need to change?

public class OrderProcessor(ICalculationService calculationService, IReceiptService receiptService) : IProcessService
{
    private readonly List<string> _auditEntries = [];
    public IReadOnlyList<string> AuditEntries => _auditEntries;

    /// <inheritdoc />
    public string Process(Order order)
    {
        var result = calculationService.CalculateTotals(order);
        var receipt = receiptService.CreateReceipt(order, result);

        _auditEntries.Add($"PROCESSED|{order.Id}|{result.Total:0.00}");
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

public interface ICalculationService
{
    public ClaclulatedResults CalculateTotals(Order order);
}

public class CalculationService : ICalculationService
{
    public ClaclulatedResults CalculateTotals(Order order)
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

        return new ClaclulatedResults(subtotal, discount, subtotal - discount);
    }
}


public interface IReceiptService
{
    public List<string> CreateReceipt(Order order, ClaclulatedResults caclulations);
}

public class ReceiptService : IReceiptService
{
    public List<string> CreateReceipt(Order order, ClaclulatedResults results)
    {
        var receipt = new List<string>
        {
            $"Order {order.Id} for {order.Customer}",
            "------------------------------"
        };

        foreach (var line in order.Lines)
            receipt.Add($"{line.Description}: {line.Quantity} x {line.UnitPrice:0.00}");

        receipt.Add($"Subtotal: {results.Subtotal:0.00}");
        receipt.Add($"Discount: {results.Discount:0.00}");
        receipt.Add($"Total: {results.Total:0.00}");

        return receipt;
    }
}