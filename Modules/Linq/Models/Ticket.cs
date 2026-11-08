namespace Modules.Linq.Models;

public sealed record Ticket(int Id, int AgentId, int ResolutionMinutes, bool IsResolved, bool IsSpam);
