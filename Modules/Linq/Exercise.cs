using Interfaces;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Modules.Linq;

public sealed class Exercise : ILearningModule
{
    public string Name => "LINQ Query Optimisation: Agent Ticket Analytics";

    private readonly AgentSummaryService agentSummaryService = new();

    public void Run()
    {
        var tickets = CreateTickets(20_000);
        var agentIds = Enumerable.Range(1, 1_000).ToArray();

        var results = agentSummaryService.GetAgentSummaries(tickets, agentIds);
        Console.WriteLine($"Summaries: {results.Count}");
        foreach (var summary in results.Take(5))
            Console.WriteLine($"{summary.AgentId}: {summary.TotalResolutionMinutes} minutes ({summary.ResolvedTicketCount} tickets)");
    }

    private static List<Ticket> CreateTickets(int count)
    {
        var random = new Random(42);
        return [.. Enumerable.Range(1, count)
            .Select(id => new Ticket(
                id,
                random.Next(1, 1_201),
                random.Next(5, 241),
                random.Next(0, 5) != 0,
                random.Next(0, 20) == 0))];
    }
}
