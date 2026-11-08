using Modules.Linq.Models;
using Modules.SOLID.Services;

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
        HashSet<int> agentIdsSet = [.. agentIds];
        var ticketLookup = tickets.Where(ticket => agentIdsSet.Contains(ticket.AgentId) && ticket.IsResolved && !ticket.IsSpam).ToLookup(ticket => ticket.AgentId);

        foreach (var agentId in agentIds)
        {
            var resolvedTickets = ticketLookup[agentId];
            var minutes = resolvedTickets.Sum(ticket => ticket.ResolutionMinutes);
            var count = resolvedTickets.Count();

            results.Add(new AgentSummary(agentId, minutes, count));
        }

        return results;
    }
}
