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
        HashSet<int> cids = [.. customerIds];
        var orderLookup = orders.Where(order => cids.Contains(order.CustomerId) && order.IsPaid && !order.IsCancelled).ToLookup(order => order.CustomerId);

        foreach (var customerId in customerIds)
        {
            var matchingOrders = orderLookup[customerId].ToList();

            var total = matchingOrders.Sum(x => x.Amount);
            var count = matchingOrders.Count;

            results.Add(new CustomerSummary(customerId, total, count));
        }

        return results;
    }
}
