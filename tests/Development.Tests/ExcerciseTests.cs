using Modules.Linq;

namespace Development.Tests;

public class LinqExerciseTests
{
    [Fact]
    public void GetCustomerSummaries_Success_ShouldReturnSummaries()
    {
        // Arrange
        var orders = CreateOrders(20_000);
        var customerIds = Enumerable.Range(1, 1_000).ToArray();
        var service = new Exercise();

        var results = service.GetCustomerSummaries(orders, customerIds);
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