using System.Diagnostics;
using Modules.Linq;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Development.Tests;

public class LinqExerciseTests
{
    [Fact]
    public void GetCustomerSummaries_Performance_Baseline()
    {
        // Arrange
        var orders = CreateOrders(200_000);
        var customerIds = Enumerable.Range(1, 20_000).ToArray();
        var service = new CustomerSummaryService();

        // Act
        var stopwatch = Stopwatch.StartNew();

        var results = service.GetCustomerSummaries(orders, customerIds);

        stopwatch.Stop();

        Console.WriteLine(
            $"Execution time: {stopwatch.Elapsed.TotalSeconds} s");

        // Assert
        Assert.Equal(20000, results.Count);
    }

    [Fact]
    public void GetCustomerSummary_Success_ShouldReturnSummary()
    {
        // Arrange
        var order = CreateOrders(1);
        var customerIds = Enumerable.Range(1, 1).ToArray();
        var service = new CustomerSummaryService();

        // Act
        var results = service.GetCustomerSummaries(order, customerIds);

        // Assert
        Assert.Single(results);
        Assert.Equal(1, results.First().CustomerId);
    }

    [Fact]
    public void GetCustomerSummaries_ExcludesUnpaidAndCancelledOrders()
    {
        // Arrange
        var orders = new List<Order>
    {
        new(1, 1, 100m, true, false),
        new(2, 1, 50m, true, false),
        new(3, 1, 30m, false, false),
        new(4, 1, 20m, true, true)
    };

        var service = new CustomerSummaryService();

        // Act
        var results = service.GetCustomerSummaries(orders, [1]);

        // Assert
        var summary = Assert.Single(results);
        Assert.Equal(1, summary.CustomerId);
        Assert.Equal(150m, summary.TotalSpent);
        Assert.Equal(2, summary.OrderCount);
    }

    [Fact]
    public void GetCustomerSummaries_DuplicateCustomerIds_ReturnsDuplicateSummaries()
    {
        // Arrange
        var orders = new List<Order>
        {
            new(1, 1, 100m, true, false)
        };

        var service = new CustomerSummaryService();

        // Act
        var results = service.GetCustomerSummaries(orders, [1, 1]);

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(results[0], results[1]);
        Assert.Equal(100m, results[0].TotalSpent);
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