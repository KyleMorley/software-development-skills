namespace Modules.SOLID.Models;

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