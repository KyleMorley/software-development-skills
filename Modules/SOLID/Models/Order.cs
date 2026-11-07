namespace Modules.SOLID.Models;

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

    public virtual decimal Fee() => 0m;
}