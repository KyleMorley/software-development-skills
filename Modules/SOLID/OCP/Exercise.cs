using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "SRP and OCP Refactoring";

    public void Run()
    {
        var bookings = new[]
        {
            new EventBooking("E-101", "Ava", "Standard", 2, false, false, false),
            new EventBooking("E-102", "Noah", "VIP", 1, true, true, false),
            new EventBooking("E-103", "Mia", "Standard", 3, true, false, false)
        };

        var processor = new EventBookingProcessor();
        foreach (var booking in bookings)
        {
            Console.WriteLine(processor.Book(booking));
            Console.WriteLine();
        }

        foreach (var entry in processor.BookingRecords)
            Console.WriteLine(entry);
    }
}

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
public class EventBookingProcessor
{
    private readonly List<string> _bookingRecords = [];
    public IReadOnlyList<string> BookingRecords => _bookingRecords;

    public string Book(EventBooking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.Id) ||
            string.IsNullOrWhiteSpace(booking.Customer) ||
            booking.Seats <= 0 ||
            (booking.TicketType != "Standard" && booking.TicketType != "VIP"))
        {
            throw new ArgumentException("Invalid event booking.");
        }

        decimal price = booking.TicketType == "VIP" ? 130m : 75m;
        decimal subtotal = price * booking.Seats;

        if (booking.Workshop)
            subtotal += 20m * booking.Seats;

        decimal discount = booking.EarlyBird
            ? Math.Round(subtotal * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;

        decimal total = subtotal - discount;

        var receipt = string.Join(Environment.NewLine,
            $"Booking {booking.Id} for {booking.Customer}",
            $"Tickets: {booking.Seats} x {booking.TicketType}",
            $"Workshop: {(booking.Workshop ? "Yes" : "No")}",
            $"Subtotal: {subtotal:0.00}",
            $"Discount: {discount:0.00}",
            $"Total: {total:0.00}");

        _bookingRecords.Add($"BOOKED|{booking.Id}|{total:0.00}");
        return receipt;
    }
}
