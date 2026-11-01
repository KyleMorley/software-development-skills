using System.Runtime.CompilerServices;
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

        var desk = new SupportDesk(new ValidationService(), new PriorityService(), new TriageService(), new ServiceLevelAgreementService(), new MessageService());
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

// Violations of SRP: What happens if we want to change error handling, team assignment, priority, responses, etc.
public class SupportDesk(IValidationService validationService,
 IPriorityService priorityService,
 ITriageService triageService,
 IServiceLevelAgreementService serviceLevelAgreementService,
 IMessageService messageService)
{
    private readonly List<string> _activityLog = [];
    public IReadOnlyList<string> ActivityLog => _activityLog;

    public string OpenTicket(SupportTicket ticket)
    {
        validationService.Validate(ticket);

        string priority = priorityService.AssignPriority(ticket);
        string team = triageService.AssignToTeam(ticket.Category);
        int responseHours = serviceLevelAgreementService.AssignResponseTimeHours(priority);

        _activityLog.Add($"OPENED|{ticket.Id}|{team}|{priority}");

        return messageService.CreateMessage(ticket, priority, team, responseHours);
    }
}


public interface IValidationService
{
    /// <summary>
    /// Validates a support ticket
    /// </summary>
    /// <param name="ticket"></param>
    public void Validate(SupportTicket ticket);
}

public class ValidationService : IValidationService
{
    /// <inheritdoc />
    public void Validate(SupportTicket ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket.Id) ||
            string.IsNullOrWhiteSpace(ticket.Customer) ||
            ticket.Impact is < 1 or > 5)
        {
            throw new ArgumentException("Invalid support ticket.");
        }
    }
}

public interface IPriorityService
{
    /// <summary>
    /// Designated ticket priority
    /// </summary>
    /// <param name="ticket">The support ticket</param>
    /// <returns>A <see cref="string"/> representing the priority of the ticket</returns>
    public string AssignPriority(SupportTicket ticket);
}
public class PriorityService : IPriorityService
{
    /// <inheritdoc />
    public string AssignPriority(SupportTicket ticket)
    {
        return ticket.Impact >= 4 || (ticket.IsPremium && ticket.Impact >= 3)
            ? "Urgent"
            : ticket.Impact >= 2 ? "Normal" : "Low";
    }
}

public interface ITriageService
{
    /// <summary>
    /// Triage ticket to team
    /// </summary>
    /// <param name="category">The category of ticket to assign</param>
    public string AssignToTeam(string category);
}

public class TriageService : ITriageService
{
    /// <inheritdoc />
    public string AssignToTeam(string category)
    {
        return category switch
        {
            "Outage" => "Operations",
            "Billing" => "Accounts",
            _ => "General"
        };
    }
}

public interface IServiceLevelAgreementService
{
    /// <summary>
    /// Assigns a response time in hours
    /// </summary>
    /// <param name="priority">Ticket priority</param>
    /// <returns></returns>
    public int AssignResponseTimeHours(string priority);
}

public class ServiceLevelAgreementService : IServiceLevelAgreementService
{
    /// <inheritdoc />
    public int AssignResponseTimeHours(string priority)
    {
        return priority switch
        {
            "Urgent" => 2,
            "Normal" => 24,
            _ => 72
        };
    }
}

public interface IMessageService
{
    /// <summary>
    /// Creates a response message to send user
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="team"></param>
    /// <param name="priority"></param>
    /// <param name="responseTimeHours"></param>
    /// <returns></returns>
    public string CreateMessage(SupportTicket ticket, string team, string priority, int responseHours);
}

public class MessageService : IMessageService
{
    /// <inheritdoc />
    public string CreateMessage(SupportTicket ticket, string team, string priority, int responseHours)
    {
        return string.Join(Environment.NewLine,
            $"Hello {ticket.Customer},",
            $"We received ticket {ticket.Id} ({ticket.Category}).",
            $"Priority: {priority}",
            $"Assigned team: {team}",
            $"Expected response: within {responseHours} hours.");
    }
}