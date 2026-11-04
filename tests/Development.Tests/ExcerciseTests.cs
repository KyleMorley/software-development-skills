using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Modules.SOLID.OCP;

namespace Development.Tests;

public class EventBookingProcessorTests
{
    [Fact]
    public void Book_Success_ShouldReturnReceiptString()
    {
        // Arrange
        var booking = new EventBooking("E-101", "Ava", "Standard", 2, false, false, false);
        var service = new EventBookingProcessor(new EventBookingValidationService(), new BookingCostService(), new ReceiptService(), new BookingRecordRepository());

        var expectedResult = string.Join(Environment.NewLine,
            "Booking E-101 for Ava",
            "Tickets: 2 x Standard",
            "Workshop: No",
            "Subtotal: 150.00",
            "Discount: 0.00",
            "Total: 150.00");

        // Act
        var receipt = service.Book(booking);

        // Assert
        Assert.Equal(expectedResult, receipt);
    }
}

public class EventBookingValidationServiceTests
{
    [Fact]
    public void ValidateBooking_InvalidSeats_ShouldThrowArgumentException()
    {
        // Arrange
        var booking = new EventBooking("E-101", "Ava", "Standard", 0, false, false, false);
        var service = new EventBookingValidationService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ValidateBooking(booking));
    }

    [Fact]
    public void ValidateBooking_InvalidType_ShouldThrowArgumentException()
    {
        // Arrange
        var booking = new EventBooking("E-101", "Ava", String.Empty, 0, false, false, false);
        var service = new EventBookingValidationService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ValidateBooking(booking));
    }

    [Fact]
    public void ValidateBooking_IsValid_ShouldNotThrowArgumentException()
    {
        // Arrange
        var booking = new EventBooking("E-101", "Ava", "Standard", 1, false, false, false);
        var service = new EventBookingValidationService();

        // Act
        service.ValidateBooking(booking);
    }
}