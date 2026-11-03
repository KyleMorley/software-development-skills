using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        var shipments = new[]
        {
            new Shipment("S-101", "Standard", 3m, false),
            new Shipment("S-102", "Express", 2m, true),
            new Shipment("S-103", "Collection", 5m, false)
        };

        var calculator = new ShippingQuoteCalculator();
        foreach (var shipment in shipments)
            Console.WriteLine($"{shipment.Id}: {calculator.Quote(shipment):0.00}");

        // New requirement: support "Overnight" delivery.
        // Charge 20.00 plus 4.00 per kilogram, with no fragile surcharge.
        // Add an Overnight example to the output once you've refactored.
    }
}

public record Shipment(
    string Id,
    string DeliveryMethod,
    decimal WeightKg,
    bool Fragile);

// OCP review: What existing code must change when a delivery method is added?
// In a real logistics platform, new carriers and delivery options arrive
// regularly; adding one should not risk changing the behaviour of the others.
public class ShippingQuoteCalculator
{
    public decimal Quote(Shipment shipment)
    {
        if (string.IsNullOrWhiteSpace(shipment.Id) ||
            shipment.WeightKg <= 0m)
        {
            throw new ArgumentException("Invalid shipment.");
        }

        decimal price;
        switch (shipment.DeliveryMethod)
        {
            case "Standard":
                price = 5m + (2m * shipment.WeightKg);
                if (shipment.Fragile)
                    price += 3m;
                break;

            case "Express":
                price = 12m + (3m * shipment.WeightKg);
                if (shipment.Fragile)
                    price += 5m;
                break;

            case "Collection":
                price = 0m;
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported delivery method: {shipment.DeliveryMethod}");
        }

        return price;
    }
}
