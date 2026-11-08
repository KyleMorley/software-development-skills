using System.Diagnostics;
using Modules.Linq.Models;
using Modules.Linq.Services;

namespace Development.Tests;

public class AgentSummaryServiceTests
{
    [Fact]
    public void GetAgentSummaries_Performance_Baseline()
    {
        // Arrange
        var tickets = CreateTickets(100_000);
        var agentIds = Enumerable.Range(1, 20_000).ToArray();
        var service = new AgentSummaryService();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var results = service.GetAgentSummaries(tickets, agentIds);
        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.Elapsed.TotalSeconds} s");
        // Assert
        Assert.Equal(20000, results.Count);
    }

    [Fact]
    public void GetAgentSummaries_NoAgentIds_ShouldReturnEmptyList()
    {
        // Arrange
        var tickets = CreateTickets(100_000);
        var service = new AgentSummaryService();

        // Act
        var results = service.GetAgentSummaries(tickets, []);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void GetAgentSummaries_NoTicketsResolvedByAgent_ShouldReturnZeroMinutesAndZeroTicketCount()
    {
        // Arrange
        IReadOnlyList<Ticket> tickets =
        [
            new Ticket(1, 999, 15, true, false),
            new Ticket(1, 999, 15, true, false),
            new Ticket(1, 5, 15, true, false),
        ];

        var service = new AgentSummaryService();

        // Act
        var results = service.GetAgentSummaries(tickets, [10]);

        // Assert
        Assert.Equal(0, results.First().TotalResolutionMinutes);
        Assert.Equal(0, results.First().ResolvedTicketCount);
    }

    [Fact]
    public void GetAgentSummaries_TicketsResolvedByAgent_ShouldReturnResolvedNonSpamTickets()
    {
        // Arrange
        IReadOnlyList<Ticket> tickets =
        [
            new Ticket(1, 10, 15, true, false),
            new Ticket(1, 10, 15, true, false),
            new Ticket(1, 999, 15, true, true),
            new Ticket(1, 5, 15, true, true),
        ];

        var service = new AgentSummaryService();

        // Act
        var results = service.GetAgentSummaries(tickets, [10]);

        // Assert
        Assert.Equal(30, results.First().TotalResolutionMinutes);
        Assert.Equal(2, results.First().ResolvedTicketCount);
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