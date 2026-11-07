using Interfaces;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ Query Optimisation: Customer Spending";

    public void Run()
    {
        var orders = CreateOrders(20_000);
        var customerIds = Enumerable.Range(1, 1_000).ToArray();

        var results = GetCustomerSummaries(orders, customerIds);
        Console.WriteLine($"Summaries: {results.Count}");
        foreach (var summary in results.Take(5))
            Console.WriteLine($"{summary.CustomerId}: {summary.TotalSpent:C} ({summary.OrderCount} orders)");
    }

    // Preserve the result contract while improving query performance.
    // The returned list contains one summary per requested customer ID, in input order.
    // Duplicate requested IDs produce duplicate summaries; customers with no qualifying
    // orders have a zero total and count. Only paid, non-cancelled orders qualify.
    public static List<CustomerSummary> GetCustomerSummaries(
        IReadOnlyList<Order> orders,
        IReadOnlyList<int> customerIds)
    {
        var results = new List<CustomerSummary>();

        foreach (var customerId in customerIds)
        {
            var matchingOrders = orders
                .Where(order => order.CustomerId == customerId)
                .ToList();

            var paidOrders = matchingOrders
                .Where(order => order.IsPaid && !order.IsCancelled)
                .ToList();

            var total = paidOrders
                .Select(order => order.Amount)
                .ToList()
                .Sum();

            var count = paidOrders.Count();

            results.Add(new CustomerSummary(customerId, total, count));
        }

        return results;
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

public sealed record Order(
    int Id,
    int CustomerId,
    decimal Amount,
    bool IsPaid,
    bool IsCancelled);

public sealed record CustomerSummary(
    int CustomerId,
    decimal TotalSpent,
    int OrderCount);
