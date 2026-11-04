using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "SRP and OCP Refactoring";

    private readonly IEnumerable<ISubtotalRules> SubtotalRules =
    [
        new TicketTypeCost(),
        new SeatsCost()
    ];

    private readonly IEnumerable<IDiscountRules> DiscountRules =
    [
        new EarlyBirdCost()
    ];

    public void Run()
    {
        var bookings = new[]
        {
            new EventBooking("E-101", "Ava", "Standard", 2, false, false, false),
            new EventBooking("E-102", "Noah", "VIP", 1, true, true, false),
            new EventBooking("E-103", "Mia", "Standard", 3, true, false, false)
        };
        var bookingRepository = new BookingRecordRepository();
        var processor = new EventBookingProcessor(new EventBookingValidationService(), new BookingCostService(SubtotalRules, DiscountRules), new ReceiptService(), bookingRepository);

        foreach (var booking in bookings)
        {
            Console.WriteLine(processor.Book(booking));
            Console.WriteLine();
        }

        foreach (var entry in bookingRepository.BookingRecords)
            Console.WriteLine(entry);
    }
}

public record BookingCosts(
    decimal Total,
    decimal Subtotal,
    decimal Discount);

public record EventBooking(
    string Id,
    string Customer,
    string TicketType,
    int Seats,
    bool Workshop,
    bool EarlyBird,
    bool IsStudent);

// Test-first refactoring: capture today's receipts, prices, records, and
// invalid-input behaviour before changing this class.
// SRP: What independent business requests could force this class to change?
// OCP: What must be edited each time a new pricing rule is introduced?
public class EventBookingProcessor(EventBookingValidationService eventBookingValidationService,
    IBookingCostService bookingCostService,
    IReceiptService receiptService,
    IBookingRecordRepository bookingRecordRepository)
{
    public string Book(EventBooking booking)
    {
        eventBookingValidationService.ValidateBooking(booking);

        var costs = bookingCostService.AddCosts(booking);
        var receipt = receiptService.GenerateReceipt(booking, costs);

        bookingRecordRepository.AddRecord(booking, costs);
        return receipt;
    }
}

public interface IBookingRecordRepository
{
    public IReadOnlyList<string> BookingRecords { get; }

    /// <summary>
    /// Persists booking to sotrage
    /// </summary>
    /// <param name="booking"></param>
    /// <param name="costs"></param>
    public void AddRecord(EventBooking booking, BookingCosts costs);
}

public class BookingRecordRepository : IBookingRecordRepository
{
    private readonly List<string> _bookingRecords = [];
    public IReadOnlyList<string> BookingRecords => _bookingRecords;

    /// <inheritdoc />
    public void AddRecord(EventBooking booking, BookingCosts costs)
    {
        _bookingRecords.Add($"BOOKED|{booking.Id}|{costs.Total:0.00}");
    }
}

public interface IReceiptService
{
    /// <summary>
    /// Generates a receipt for the booking
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    public string GenerateReceipt(EventBooking booking, BookingCosts costs);
}

public class ReceiptService : IReceiptService
{
    /// <inheritdoc />
    public string GenerateReceipt(EventBooking booking, BookingCosts costs)
    {
        return string.Join(Environment.NewLine,
            $"Booking {booking.Id} for {booking.Customer}",
            $"Tickets: {booking.Seats} x {booking.TicketType}",
            $"Workshop: {(booking.Workshop ? "Yes" : "No")}",
            $"Subtotal: {costs.Subtotal:0.00}",
            $"Discount: {costs.Discount:0.00}",
            $"Total: {costs.Total:0.00}");
    }
}

public interface IBookingCostService
{
    /// <summary>
    /// Adds all associated costs for a booking
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    public BookingCosts AddCosts(EventBooking booking);
}

public class BookingCostService(
    IEnumerable<ISubtotalRules> subtotalRules,
    IEnumerable<IDiscountRules> discountRules) : IBookingCostService
{
    /// <inheritdoc />
    public BookingCosts AddCosts(EventBooking booking)
    {
        var subtotal = 0m;
        var discount = 0m;

        foreach(var rule in subtotalRules)
        {
            subtotal += rule.AddCost(booking, subtotal);
        }

        foreach(var rule in discountRules)
        {
            discount += rule.AddCost(booking, subtotal);
        }

        decimal total = subtotal - discount;

        return new(total, subtotal, discount);
    }
}

public interface ISubtotalRules
{
    /// <summary>
    ///  Add costs
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    public decimal AddCost(EventBooking booking, decimal currentCost);
}

public interface IDiscountRules
{
    /// <summary>
    ///  Add costs
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    public decimal AddCost(EventBooking booking, decimal currentCost);
}

public class TicketTypeCost : ISubtotalRules
{
    /// <inheritdoc />
    public decimal AddCost(EventBooking booking, decimal currentCost)
    {
        return booking.TicketType == "VIP" ? 130m : 75m;
    }
}

public class SeatsCost : ISubtotalRules
{
    /// <inheritdoc />
    public decimal AddCost(EventBooking booking, decimal currentCost)
    {
        return currentCost * booking.Seats;
    }
}

public class WorkshopCost : ISubtotalRules
{
    /// <inheritdoc />
    public decimal AddCost(EventBooking booking, decimal currentCost)
    {
        return booking.Workshop ? currentCost += 20m * booking.Seats : 0m;
    }
}

public class EarlyBirdCost : IDiscountRules
{
    /// <inheritdoc />
    public decimal AddCost(EventBooking booking, decimal currentCost)
    {
        return booking.EarlyBird
            ? Math.Round(currentCost * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;
    }
}

public interface IEventBookingValidationService
{
    /// <summary>
    /// Validates an event booking
    /// </summary>
    /// <param name="booking"></param>
    public void ValidateBooking(EventBooking booking);
}

public class EventBookingValidationService : IEventBookingValidationService
{
    /// <inheritdoc />
    public void ValidateBooking(EventBooking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.Id) ||
            string.IsNullOrWhiteSpace(booking.Customer) ||
            booking.Seats <= 0 ||
            (booking.TicketType != "Standard" && booking.TicketType != "VIP"))
        {
            throw new ArgumentException("Invalid event booking.");
        }
    }
}