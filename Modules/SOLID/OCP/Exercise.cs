using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    private readonly IEnumerable<ICarRentalRules> Rules =
    [
        new DriverAgeRuleService(),
        new AirportPickupeRuleService(),
        new ChildSeatRuleService(),
        new GPSRequestRuleService()
    ];

    public void Run()
    {
        var rentals = new[]
        {
            new CarRental("R-101", 2, 30, false, false, false),
            new CarRental("R-102", 3, 22, true, true, false),
            new CarRental("R-103", 1, 45, false, true, true)
        };

        var calculator = new RentalPriceCalculator(new RentalValidationService(), Rules);
        foreach (var rental in rentals)
            Console.WriteLine($"{rental.Id}: {calculator.Calculate(rental):0.00}");
    }
}

public record CarRental(
    string Id,
    int Days,
    int DriverAge,
    bool AirportPickup,
    bool ChildSeat,
    bool GpsRequested);

// OCP exercise: First protect the existing behaviour with regression tests.
// Refactor without changing their expected results.
// Only then add the NEW GPS charge described in the exercise requirements.
public class RentalPriceCalculator(IRentalValidationService rentalValidationService, IEnumerable<ICarRentalRules> rules)
{
    public decimal Calculate(CarRental rental)
    {
        rentalValidationService.ValidateRental(rental);

        decimal total = rental.Days * 40m;

        foreach(var rule in rules)
        {
            total += rule.AddCost(rental);
        }

        return total;
    }
}

public interface IRentalValidationService
{
    /// <summary>
    /// Verifies the rental has correct information
    /// </summary>
    /// <param name="rental"></param>
    public void ValidateRental(CarRental rental);
}

public class RentalValidationService : IRentalValidationService
{
    public void ValidateRental(CarRental rental)
    {
        if (string.IsNullOrWhiteSpace(rental.Id) ||
            rental.Days <= 0 ||
            rental.DriverAge < 18)
        {
            throw new ArgumentException("Invalid rental.");
        }
    }
}

public interface ICarRentalRules
{
    /// <summary>
    /// Adds a cost
    /// </summary>
    /// <param name="rental"></param>
    /// <returns></returns>
    public decimal AddCost(CarRental rental);
}

public class DriverAgeRuleService : ICarRentalRules
{
    public decimal AddCost(CarRental rental)
    {
        return rental.DriverAge < 25 ? rental.Days * 12m : 0m;
    }
}

public class AirportPickupeRuleService : ICarRentalRules
{
    public decimal AddCost(CarRental rental)
    {
        return rental.AirportPickup ? 25m : 0m;
    }
}

public class ChildSeatRuleService : ICarRentalRules
{
    public decimal AddCost(CarRental rental)
    {
        return rental.ChildSeat ? rental.Days * 8m : 0m;
    }
}

public class GPSRequestRuleService : ICarRentalRules
{
    public decimal AddCost(CarRental rental)
    {
        return rental.GpsRequested ? rental.Days * 6m : 0m;
    }
}