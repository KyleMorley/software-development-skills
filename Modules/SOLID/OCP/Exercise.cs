using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        var rentals = new[]
        {
            new CarRental("R-101", 2, 30, false, false, false),
            new CarRental("R-102", 3, 22, true, true, false),
            new CarRental("R-103", 1, 45, false, true, true)
        };

        var calculator = new RentalPriceCalculator();
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
public class RentalPriceCalculator
{
    public decimal Calculate(CarRental rental)
    {
        if (string.IsNullOrWhiteSpace(rental.Id) ||
            rental.Days <= 0 ||
            rental.DriverAge < 18)
        {
            throw new ArgumentException("Invalid rental.");
        }

        decimal total = rental.Days * 40m;

        if (rental.DriverAge < 25)
            total += rental.Days * 12m;

        if (rental.AirportPickup)
            total += 25m;

        if (rental.ChildSeat)
            total += rental.Days * 8m;

        return total;
    }
}
