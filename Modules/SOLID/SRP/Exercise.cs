using Interfaces;

namespace Modules.SOLID.SRP;

public class Exercise : ILearningModule
{
    public string Name => "Single Responsibility Principle";

    public void Run()
    {
        var tickets = new[]
        {
            new SupportTicket("T-101", "Ava", "Outage", 5, true),
            new SupportTicket("T-102", "Noah", "Billing", 2, false),
            new SupportTicket("T-103", "Mia", "Question", 3, false)
        };

        var desk = new SupportDesk();
        foreach (var ticket in tickets)
        {
            Console.WriteLine(desk.OpenTicket(ticket));
            Console.WriteLine();
        }

        foreach (var entry in desk.ActivityLog)
            Console.WriteLine(entry);
    }
}

public record SupportTicket(
    string Id,
    string Customer,
    string Category,
    int Impact,
    bool IsPremium);

// SRP review questions: Who would ask for this code to change, and why?
// Could one business rule change while the others stay the same?
// Count independent reasons to change, not methods or lines of code.
//
// Real-world motivation: When a support company updates its policies,
// unrelated parts of the system should be less likely to break or need retesting.
// Clear ownership also lets different teams make changes with less coordination.
public class SupportDesk
{
    private readonly List<string> _activityLog = [];
    public IReadOnlyList<string> ActivityLog => _activityLog;

    public string OpenTicket(SupportTicket ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket.Id) ||
            string.IsNullOrWhiteSpace(ticket.Customer) ||
            ticket.Impact is < 1 or > 5)
        {
            throw new ArgumentException("Invalid support ticket.");
        }

        string priority = ticket.Impact >= 4 || (ticket.IsPremium && ticket.Impact >= 3)
            ? "Urgent"
            : ticket.Impact >= 2 ? "Normal" : "Low";

        string team = ticket.Category switch
        {
            "Outage" => "Operations",
            "Billing" => "Accounts",
            _ => "General"
        };

        int responseHours = priority switch
        {
            "Urgent" => 2,
            "Normal" => 24,
            _ => 72
        };

        var message = string.Join(Environment.NewLine,
            $"Hello {ticket.Customer},",
            $"We received ticket {ticket.Id} ({ticket.Category}).",
            $"Priority: {priority}",
            $"Assigned team: {team}",
            $"Expected response: within {responseHours} hours.");

        _activityLog.Add($"OPENED|{ticket.Id}|{team}|{priority}");
        return message;
    }
}
