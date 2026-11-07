using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class CustomerSummaryService
{
    // Return one summary per requested customer ID, in input order.
    // Duplicate requested IDs produce duplicate summaries; customers with no qualifying
    // orders have a zero total and count. Only paid, non-cancelled orders qualify.
    public List<CustomerSummary> GetCustomerSummaries(
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
}
