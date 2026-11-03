using System.Security.Cryptography.X509Certificates;
using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public List<IShippingDeliveryCalculator> CalculationStrategies =
    [
        new StandardDeliveryService(),
        new ExpresstDeliveryService(),
        new CollectionService(),
        new OvernightDeliveryService()
    ];

    public void Run()
    {
        Dictionary<string, IShippingDeliveryCalculator> services = new()
        {
            ["Standard"] = new StandardDeliveryService(),
            ["Express"] = new ExpresstDeliveryService(),
            ["Collection"] = new CollectionService(),
            ["Overnight"] = new OvernightDeliveryService(),
        };

        

        var shipments = new[]
        {
            new Shipment("S-101", "Standard", 3m, false),
            new Shipment("S-102", "Express", 2m, true),
            new Shipment("S-103", "Collection", 5m, false),
            new Shipment("S-104", "Express", 2m, false)
        };

        var calculator = new ShippingQuoteCalculator(new ShipmentValdationService(), CalculationStrategies,  services, shipments);
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
public class ShippingQuoteCalculator(IShipmentValidationService shipmentValidationService, List<IShippingDeliveryCalculator> shippingDeliveryCalculators,
Dictionary<string, IShippingDeliveryCalculator> services, Shipment[] shipments)
{
    public void ProcessShipments()
    {
        foreach (var shipment in shipments)
        {
            if(!services.TryGetValue(shipment.DeliveryMethod, out var shippingDeliveryCalculator))
            {
                throw new NotSupportedException(
                    $"Unsupported delivery method: {shipment.DeliveryMethod}");
            }

            Console.WriteLine($"{shipment.Id}: {Quote(shipment):0.00}");
        }
    }
    
    public decimal Quote(Shipment shipment)
    {
        shipmentValidationService.ValidateShipment(shipment);

        return shippingDeliveryCalculator.CalculateDelivery(shipment);
    }
}

public interface IShippingDeliveryCalculator
{
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