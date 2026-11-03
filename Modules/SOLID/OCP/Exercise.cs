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
        var shipments = new[]
        {
            new Shipment("S-101", "Standard", 3m, false),
            new Shipment("S-102", "Express", 2m, true),
            new Shipment("S-103", "Collection", 5m, false),
            new Shipment("S-104", "Express", 2m, false)
        };

        var calculator = new ShippingQuoteCalculator(new ShipmentValdationService(), CalculationStrategies);
        foreach (var shipment in shipments)
        {
            Console.WriteLine($"{shipment.Id}: {calculator.Quote(shipment):0.00}");
        }
    }
}

public record Shipment(
    string Id,
    string DeliveryMethod,
    decimal WeightKg,
    bool Fragile);

public class ShippingQuoteCalculator(IShipmentValidationService shipmentValidationService,
    List<IShippingDeliveryCalculator> shippingDeliveryCalculators)
{
    public decimal Quote(Shipment shipment)
    {
        shipmentValidationService.ValidateShipment(shipment);
        var shippingDeliveryCalculator = shippingDeliveryCalculators.FirstOrDefault(method => method.DeliveryMethod == shipment.DeliveryMethod) ?? throw new ArgumentException("Not supported delivert method.");
        return shippingDeliveryCalculator.CalculateDelivery(shipment);
    }
}

public interface IShippingDeliveryCalculator
{
    /// <summary>
    /// Delivery type readonly property
    /// </summary>
    public string DeliveryMethod { get; }

    /// <summary>
    /// Calculate the shipment cost
    /// </summary>
    /// <returns></returns>
    public decimal CalculateDelivery(Shipment shipment);
}

public class StandardDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public string DeliveryMethod => "Standard";

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
    public string DeliveryMethod => "Collection";

    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        return 0m;
    }
}

public class OvernightDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public string DeliveryMethod => "Overnight";

    /// <inheritdoc />
    public decimal CalculateDelivery(Shipment shipment)
    {
        return 20m + (4m * shipment.WeightKg);
    }
}

public class ExpresstDeliveryService : IShippingDeliveryCalculator
{
    /// <inheritdoc />
    public string DeliveryMethod => "Express";

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