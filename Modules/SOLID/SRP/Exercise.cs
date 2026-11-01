using Interfaces;

namespace Modules.SOLID.SRP;

public class Exercise : ILearningModule
{
    public string Name => "Single Responsibility Principle";

    public void Run()
    {
        var renewals = new[]
        {
            new Membership("M-101", "Ava", "Standard", 2, false),
            new Membership("M-102", "Noah", "Premium", 5, true),
            new Membership("M-103", "Mia", "Standard", 0, true)
        };

        var manager = new MembershipRenewalManager();
        foreach (var member in renewals)
        {
            Console.WriteLine(manager.Renew(member));
            Console.WriteLine();
        }

        foreach (var entry in manager.RenewalRecords)
            Console.WriteLine(entry);
    }
}

public record Membership(
    string Id,
    string Name,
    string Plan,
    int YearsAsMember,
    bool AutoRenew);

// SRP review: Who would request each change, and could those requests arrive independently?
// In a real subscription business, unrelated policy and communication updates should
// not require repeatedly editing and retesting the same large class.
public class MembershipRenewalManager
{
    private readonly List<string> _renewalRecords = [];
    public IReadOnlyList<string> RenewalRecords => _renewalRecords;

    public string Renew(Membership member)
    {
        if (string.IsNullOrWhiteSpace(member.Id) ||
            string.IsNullOrWhiteSpace(member.Name) ||
            member.YearsAsMember < 0 ||
            (member.Plan != "Standard" && member.Plan != "Premium"))
        {
            throw new ArgumentException("Invalid membership.");
        }

        decimal annualFee = member.Plan == "Premium" ? 120m : 60m;
        decimal loyaltyDiscount = member.YearsAsMember >= 5 ? 0.15m : 0m;
        decimal renewalDiscount = member.AutoRenew ? 0.05m : 0m;
        decimal finalFee = Math.Round(
            annualFee * (1m - loyaltyDiscount - renewalDiscount),
            2,
            MidpointRounding.AwayFromZero);

        string status = member.AutoRenew ? "Automatic" : "Manual";
        string notice = string.Join(Environment.NewLine,
            $"Membership renewal for {member.Name}",
            $"Member: {member.Id}",
            $"Plan: {member.Plan}",
            $"Renewal: {status}",
            $"Fee due: ${finalFee:0.00}");

        _renewalRecords.Add(
            $"RENEWAL|{member.Id}|{member.Plan}|{finalFee:0.00}|{status}");

        return notice;
    }
}
