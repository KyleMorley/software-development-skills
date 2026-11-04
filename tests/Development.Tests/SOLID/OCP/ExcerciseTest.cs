using Modules.SOLID.OCP;

namespace Development.Tests.SOLID.OCP;

public class RentalPriceCalculatorTests
{
    private readonly IEnumerable<ICarRentalRules> Rules =
    [
        new DriverAgeRuleService(),
        new AirportPickupeRuleService(),
        new ChildSeatRuleService()
    ];

    [Fact]
    public void RentalPriceCalculator_Calculate_ShouldReturnCorrectTotal()
    {
        // Arrange
        var rentalA = new CarRental("R-101", 2, 30, false, false, false);
        var rentalB = new CarRental("R-102", 3, 22, true, true, false);
        var rentalC = new CarRental("R-103", 1, 45, false, true, false);

        var service = new RentalPriceCalculator(new RentalValidationService(), Rules);

        // Act
        var resultA = service.Calculate(rentalA);
        var resultB = service.Calculate(rentalB);
        var resultC = service.Calculate(rentalC);

        // Assert
        Assert.Equal(80m, resultA);
        Assert.Equal(205m, resultB);
        Assert.Equal(48m, resultC);
    }
}


public class RentalValidationServiceTests
{
    [Fact]
    public static void ValidateRental_InvalidDays_ShouldThrowArgumentException()
    {
        // Arrange
        var rental = new CarRental("Test", 0, 10, false, false, false);
        var service = new RentalValidationService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ValidateRental(rental));
    }

    [Fact]
    public static void ValidateRental_InvalidAge_ShouldThrowArgumentException()
    {
        // Arrange
        var rental = new CarRental("Test", 3, 10, false, false, false);
        var service = new RentalValidationService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ValidateRental(rental));
    }
}