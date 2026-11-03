using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        var visits = new[]
        {
            new HotelStay("H-101", 2, 180m, false, false, false),
            new HotelStay("H-102", 5, 620m, true, true, false),
            new HotelStay("H-103", 3, 250m, true, false, true)
        };

        var calculator = new RewardPointsCalculator();
        foreach (var stay in visits)
        {
            var reward = calculator.Calculate(stay);
            Console.WriteLine($"{stay.Id}: {reward.Points} points ({reward.Tier})");
        }

        // New requirement: a stay booked at least 30 days in advance
        // earns an extra 12 points. Add a sample that exercises this rule.
    }
}

public record HotelStay(
    string Id,
    int Nights,
    decimal TotalCost,
    bool LoyaltyMember,
    bool WeekendStay,
    bool BookedEarly);

public record RewardResult(int Points, string Tier);

// OCP question: When marketing introduces another way to earn points,
// can you extend the system without editing the calculation algorithm?
// Real-world benefit: Promotional rules change frequently; new promotions
// should not risk breaking the existing loyalty rewards.
public class RewardPointsCalculator
{
    public RewardResult Calculate(HotelStay stay)
    {
        if (string.IsNullOrWhiteSpace(stay.Id) ||
            stay.Nights <= 0 ||
            stay.TotalCost < 0m)
        {
            throw new ArgumentException("Invalid hotel stay.");
        }

        int points = stay.Nights * 5;

        if (stay.LoyaltyMember)
            points += 20;

        if (stay.WeekendStay)
            points += 10;

        if (stay.TotalCost >= 500m)
            points += 25;

        points = Math.Min(points, 100);

        string tier = points switch
        {
            >= 60 => "Gold",
            >= 30 => "Silver",
            _ => "Bronze"
        };

        return new RewardResult(points, tier);
    }
}
