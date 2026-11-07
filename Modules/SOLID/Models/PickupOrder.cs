namespace Modules.SOLID.Models;

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