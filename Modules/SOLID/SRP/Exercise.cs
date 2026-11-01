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

        var manager = new MembershipRenewalManager(
            new MembershipValidationService(),
            new FeeService(),
            new MembershipStatusService(),
            new NoticeService(),
            new RenewMembershipRecordService());

        foreach (var member in renewals)
        {
            Console.WriteLine(manager.Renew(member));
            Console.WriteLine();
        }
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
public class MembershipRenewalManager(IMembershipValidationService membershipValidationService,
IFeeService feeService,
IMembershipStatusService membershipStatusService,
INoticeService noticeService,
IRenewMembershipRecordService renewMembershipRecordService)
{
    public string Renew(Membership member)
    {
        membershipValidationService.ValidateMember(member);
        decimal finalFee = feeService.CalculateFee(member);
        string status = membershipStatusService.GetMemberStatus(member);
        string notice = noticeService.GenerateMembershipNotice(member, status, finalFee);

        renewMembershipRecordService.AddRecord(member, finalFee, status);
        renewMembershipRecordService.PrintRecords();
        return notice;
    }
}

public interface IRenewMembershipRecordService
{
    /// <summary>
    /// Adds record for bookkeeping to maintain member information
    /// </summary>
    /// <param name="member"></param>
    /// <param name="fee"></param>
    /// <param name="status"></param>
    public void AddRecord(Membership member, decimal fee, string status);

    /// <summary>
    /// Prints the list of records
    /// </summary>
    public void PrintRecords();
}

public class RenewMembershipRecordService : IRenewMembershipRecordService
{
    /// <summary>
    /// Internal list of renewal records
    /// </summary>
    private readonly List<string> _renewalRecords = [];

    /// <inheritdoc />
    public void AddRecord(Membership member, decimal fee, string status)
    {
        _renewalRecords.Add(
            $"RENEWAL|{member.Id}|{member.Plan}|{fee:0.00}|{status}");
    }

    /// <inheritdoc />
    public void PrintRecords()
    {
        foreach (var entry in _renewalRecords)
            Console.WriteLine(entry);
    }
}

public interface IMembershipValidationService
{
    /// <summary>
    /// Validates memebrship properties
    /// </summary>
    /// <param name="member">An ofject of type Membership to validate</param>
    public void ValidateMember(Membership member);
}

public class MembershipValidationService : IMembershipValidationService
{
    /// <inheritdoc />
    public void ValidateMember(Membership member)
    {
        if (string.IsNullOrWhiteSpace(member.Id) ||
            string.IsNullOrWhiteSpace(member.Name) ||
            member.YearsAsMember < 0 ||
            (member.Plan != "Standard" && member.Plan != "Premium"))
        {
            throw new ArgumentException("Invalid membership.");
        }
    }
}

public interface IFeeService
{
    /// <summary>
    /// Calculates a fee for a type of membership
    /// </summary>
    /// <param name="member">The meber to calculate the fee from</param>
    /// <returns></returns>
    public decimal CalculateFee(Membership member);
}

public class FeeService : IFeeService
{
    /// <inheritdoc />
    public decimal CalculateFee(Membership member)
    {
        decimal annualFee = member.Plan == "Premium" ? 120m : 60m;
        decimal loyaltyDiscount = member.YearsAsMember >= 5 ? 0.15m : 0m;
        decimal renewalDiscount = member.AutoRenew ? 0.05m : 0m;
        return Math.Round(
            annualFee * (1m - loyaltyDiscount - renewalDiscount),
            2,
            MidpointRounding.AwayFromZero);
    }
}

public interface IMembershipStatusService
{
    /// <summary>
    /// Gets a members membership status
    /// </summary>
    /// <param name="member">The member to get the membership status for</param>
    /// <returns></returns>
    public string GetMemberStatus(Membership member);
}

public class MembershipStatusService : IMembershipStatusService

{
    /// <inheritdoc />
    public string GetMemberStatus(Membership member)
    {
        return member.AutoRenew ? "Automatic" : "Manual";
    }
}

public interface INoticeService
{
    /// <summary>
    /// Generates a notice for the member
    /// </summary>
    /// <param name="member"></param>
    /// <param name="status"></param>
    /// <param name="fee"></param>
    /// <returns></returns>
    public string GenerateMembershipNotice(Membership member, string status, decimal fee);
}

public class NoticeService : INoticeService
{
    /// <inheritdoc />
    public string GenerateMembershipNotice(Membership member, string status, decimal fee)
    {
        return string.Join(Environment.NewLine,
            $"Membership renewal for {member.Name}",
            $"Member: {member.Id}",
            $"Plan: {member.Plan}",
            $"Renewal: {status}",
            $"Fee due: ${fee:0.00}");
    }
}