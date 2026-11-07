using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "SOLID Refactoring: Order Fulfilment";

    public void Run()
    {
        var processor = new OrderProcessor();
        var orders = new Order[]
        {
            new DeliveryOrder("A-101", "Ava", 2, "Standard", "Metro"),
            new PickupOrder("A-102", "Noah", 1, "Express", "Store"),
            new DeliveryOrder("A-103", "Mia", 3, "Express", "Regional")
        };

        foreach (var order in orders)
        {
            Console.WriteLine(processor.Process(order));
            Console.WriteLine();
        }

        foreach (var record in processor.Records)
            Console.WriteLine(record);
    }
}

public abstract class Order(
    string id,
    string customer,
    int quantity,
    string serviceLevel,
    string destination)
{
    public string Id { get; } = id;
    public string Customer { get; } = customer;
    public int Quantity { get; } = quantity;
    public string ServiceLevel { get; } = serviceLevel;
    public string Destination { get; } = destination;

    public virtual decimal DeliveryFee() => 0m;
}

public sealed class DeliveryOrder(
    string id,
    string customer,
    int quantity,
    string serviceLevel,
    string destination)
    : Order(id, customer, quantity, serviceLevel, destination)
{
    public override decimal DeliveryFee() => Destination switch
    {
        "Metro" => 8m,
        "Regional" => 18m,
        _ => throw new ArgumentException("Unknown delivery destination.")
    };
}

public sealed class PickupOrder(
    string id,
    string customer,
    int quantity,
    string serviceLevel,
    string destination)
    : Order(id, customer, quantity, serviceLevel, destination)
{
    public override decimal DeliveryFee() =>
        throw new NotSupportedException("Pickup orders do not support delivery fees.");
}

public class OrderProcessor
{
    private readonly List<string> _records = [];
    private readonly ConsoleOrderNotifier _notifier = new();

    public IReadOnlyList<string> Records => _records;

    public string Process(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.Id) ||
            string.IsNullOrWhiteSpace(order.Customer) ||
            order.Quantity <= 0 ||
            (order.ServiceLevel != "Standard" && order.ServiceLevel != "Express"))
        {
            throw new ArgumentException("Invalid order.");
        }

        var unitPrice = order.ServiceLevel switch
        {
            "Standard" => 25m,
            "Express" => 40m,
            _ => throw new ArgumentException("Unknown service level.")
        };

        var subtotal = unitPrice * order.Quantity;
        decimal fulfilmentFee;

        if (order is PickupOrder)
        {
            fulfilmentFee = 0m;
        }
        else if (order is DeliveryOrder)
        {
            fulfilmentFee = order.DeliveryFee();
        }
        else
        {
            throw new ArgumentException("Unknown order type.");
        }

        var total = subtotal + fulfilmentFee;
        var receipt = string.Join(Environment.NewLine,
            $"Order {order.Id} for {order.Customer}",
            $"Service: {order.ServiceLevel}",
            $"Quantity: {order.Quantity}",
            $"Subtotal: {subtotal:0.00}",
            $"Fulfilment: {fulfilmentFee:0.00}",
            $"Total: {total:0.00}");

        _records.Add($"PROCESSED|{order.Id}|{total:0.00}");
        _notifier.Notify(order.Customer, order.Id);
        return receipt;
    }
}

public class ConsoleOrderNotifier
{
    public void Notify(string customer, string orderId)
    {
        Console.WriteLine($"NOTIFY|{customer}|{orderId}");
    }
}
