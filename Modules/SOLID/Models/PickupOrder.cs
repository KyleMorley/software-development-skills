namespace Modules.SOLID.Models;

public sealed class PickupOrder(
    string id,
    string customer,
    int quantity,
    string serviceLevel,
    string destination)
    : Order(id, customer, quantity, serviceLevel, destination)
{
    public override decimal Fee() => 0m;
}