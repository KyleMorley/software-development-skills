using Modules.Linq.Models;

namespace Modules.Linq.Services;

public sealed class AgentSummaryService
{
    // Return one summary per requested agent ID, in input order.
    // Duplicate IDs produce duplicate summaries; agents without qualifying
    // tickets receive zero minutes and zero count. Only resolved, non-spam tickets qualify.
    public List<AgentSummary> GetAgentSummaries(
        IReadOnlyList<Ticket> tickets,
        IReadOnlyList<int> agentIds)
    {
        var results = new List<AgentSummary>();

        foreach (var agentId in agentIds)
        {
            var assignedTickets = tickets
                .Where(ticket => ticket.AgentId == agentId)
                .ToList();

            var resolvedTickets = assignedTickets
                .Where(ticket => ticket.IsResolved && !ticket.IsSpam)
                .ToList();

            var minutes = resolvedTickets
                .Select(ticket => ticket.ResolutionMinutes)
                .ToList()
                .Sum();

            var count = resolvedTickets.Count();

            results.Add(new AgentSummary(agentId, minutes, count));
        }

        return results;
    }
}
