using System.Security.Cryptography.X509Certificates;
using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    // HINT: This list is fine. You don't need a dictionary as well.
    // Each strategy just needs to tell us which DeliveryMethod it supports.
    public List<IShippingDeliveryCalculator> CalculationStrategies =
    [
        new StandardDeliveryService(),
        new ExpresstDeliveryService(),
        new CollectionService(),
        new OvernightDeliveryService()
    ];

    public void Run()
    {
        var shipments = new[]
        {
            new Shipment("S-101", "Standard", 3m, false),
            new Shipment("S-102", "Express", 2m, true),
            new Shipment("S-103", "Collection", 5m, false),
            new Shipment("S-104", "Express", 2m, false)
        };

        // HINT: Let the calculator pick the strategy; this runner can just
        // print each quote. 'services' below no longer exists.
        var calculator = new ShippingQuoteCalculator(new ShipmentValdationService(), CalculationStrategies, shipments);
        foreach (var shipment in shipments)
        {
            if(!services.TryGetValue(shipment.DeliveryMethod, out var shippingDeliveryCalculator))
            {
                throw new NotSupportedException(
                    $"Unsupported delivery method: {shipment.DeliveryMethod}");
            }

            Console.WriteLine($"{shipment.Id}: {calculator.Quote(shipment):0.00}");
        }
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
public class ShippingQuoteCalculator(IShipmentValidationService shipmentValidationService,
    List<IShippingDeliveryCalculator> shippingDeliveryCalculators,
    Shipment[] shipments)
{
    // HINT: You can simplify by keeping shipment loops in Run(), rather
    // than duplicating them here. Quote() can handle one shipment at a time.
    public void ProcessShipments()
    {
        foreach (var shipment in shipments)
        {
            // HINT: A List has no TryGetValue or string key lookup.
            // After each strategy exposes its supported method, try searching
            // the list with FirstOrDefault(...) or a simple foreach loop.
            if(!shippingDeliveryCalculators.(shipment.DeliveryMethod, out var shippingDeliveryCalculator))
            {
                throw new NotSupportedException(
                    $"Unsupported delivery method: {shipment.DeliveryMethod}");
            }

            Console.WriteLine($"{shipment.Id}: {Quote(shipment):0.00}");
        }
    }

    // HINT: 'shippingDeliveryCalculator' is not defined in Quote().
    // Find the matching strategy here, then call CalculateDelivery(shipment).
    // If there is no match, preserve the NotSupportedException behaviour.
    public decimal Quote(Shipment shipment)
    {
        shipmentValidationService.ValidateShipment(shipment);

        return shippingDeliveryCalculator.CalculateDelivery(shipment);
    }
}

public interface IShippingDeliveryCalculator
{
    // HINT: What read-only property could identify a strategy as
    // "Standard", "Express", "Collection" or "Overnight"? Each class
    // could supply its own value through this common interface.
    /// <summary>
    /// Calculate the shipment cost
    /// </summary>
    /// <returns></returns>
    public decimal CalculateDelivery(Shipment shipment);
}

public class StandardDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        var price = 5m + (2m * shipment.WeightKg);
        if (shipment.Fragile)
            price += 3m;

        return price;
    }
}

public class CollectionService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        return 0m;
    }
}

public class OvernightDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        return 20m + (4m * shipment.WeightKg);
    }
}

public class ExpresstDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        var price = 12m + (3m * shipment.WeightKg);
        if (shipment.Fragile)
            price += 5m;

        return price;
    }
}

public interface IShipmentValidationService
{
    /// <summary>
    /// Validates a shipment
    /// </summary>
    /// <param name="shipment" cref="Shipment">The shipment to validate</param>
    public void ValidateShipment(Shipment shipment);
}

public class ShipmentValdationService : IShipmentValidationService
{
    /// <inheritdoc />
    public void ValidateShipment(Shipment shipment)
    {
        if (string.IsNullOrWhiteSpace(shipment.Id) ||
            shipment.WeightKg <= 0m)
        {
            throw new ArgumentException("Invalid shipment.");
        }
    }
}