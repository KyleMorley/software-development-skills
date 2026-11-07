using Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ Query Optimisation: Customer Spending";

    private readonly CustomerSummaryService customerSummaryService = new();

    public void Run()
    {
        var orders = CreateOrders(20_000);
        var customerIds = Enumerable.Range(1, 1_000).ToArray();

        var results = customerSummaryService.GetCustomerSummaries(orders, customerIds);
        Console.WriteLine($"Summaries: {results.Count}");
        foreach (var summary in results.Take(5))
            Console.WriteLine($"{summary.CustomerId}: {summary.TotalSpent:C} ({summary.OrderCount} orders)");
    }

    private static List<Order> CreateOrders(int count)
    {
        var random = new Random(42);
        return Enumerable.Range(1, count)
            .Select(id => new Order(
                id,
                random.Next(1, 1_201),
                random.Next(100, 20_000) / 100m,
                random.Next(0, 5) != 0,
                random.Next(0, 20) == 0))
            .ToList();
    }
}
