using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    private readonly List<IPointsAssignment> Rules =
    [
        new LoyaltyMember(),
        new WeekendStay(),
        new TotalCost(),
        new AdvancedBooking()
    ];

    public void Run()
    {
        var visits = new[]
        {
            new HotelStay("H-101", 2, 180m, false, false, false),
            new HotelStay("H-102", 5, 620m, true, true, false),
            new HotelStay("H-103", 3, 250m, true, false, true)
        };

        var calculator = new RewardPointsCalculator(Rules);
        foreach (var stay in visits)
        {
            var reward = calculator.Calculate(stay);
            Console.WriteLine($"{stay.Id}: {reward.Points} points ({reward.Tier})");
        }
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

public class RewardPointsCalculator(List<IPointsAssignment> Rules)
{
    public RewardResult Calculate(HotelStay stay)
    {
        ValidateHotelStayRecord(stay);

        int points = stay.Nights * 5;

        foreach(var rule in Rules)
        {
            rule.AddPoints(stay);
        }

        points = Math.Min(points, 100);

        string tier = GetLoyaltyTier(points);

        return new RewardResult(points, tier);
    }

    /// <summary>
    /// Assigns tier based on points
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private static string GetLoyaltyTier(int points)
    {
        return points switch
        {
            >= 60 => "Gold",
            >= 30 => "Silver",
            _ => "Bronze"
        };
    }

    /// <summary>
    /// Validates the hotel stay record
    /// </summary>
    /// <param name="stay"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void ValidateHotelStayRecord(HotelStay stay)
    {
        if (string.IsNullOrWhiteSpace(stay.Id) ||
            stay.Nights <= 0 ||
            stay.TotalCost < 0m)
        {
            throw new ArgumentException("Invalid hotel stay.");
        }
    }
}


public interface IPointsAssignment
{
    /// <summary>
    /// Add points
    /// </summary>
    public int AddPoints(HotelStay stay);
}

public class LoyaltyMember : IPointsAssignment
{
    /// <inheritdoc />
    public int AddPoints(HotelStay stay) => stay.LoyaltyMember ? 20 : 0;
}

public class WeekendStay : IPointsAssignment
{
    /// <inheritdoc />
    public int AddPoints(HotelStay stay) => stay.WeekendStay ? 10 : 0;
}

public class TotalCost : IPointsAssignment
{
    /// <inheritdoc />
    public int AddPoints(HotelStay stay) => stay.TotalCost >= 500m ? 25 : 0;
}

public class AdvancedBooking : IPointsAssignment
{
    /// <inheritdoc />
    public int AddPoints(HotelStay stay) => stay.BookedEarly ? 12 : 0;
}