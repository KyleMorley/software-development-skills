using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;
using Modules.SOLID.OCP;
using Modules.SOLID.Repositories;
using Modules.SOLID.Services;

namespace Development.Tests;

public class OrderProcessorTests
{
     public IReadOnlyDictionary<string, IServiceLevelPolicy> Policies = new
        Dictionary<string, IServiceLevelPolicy>
    {
        ["Standard"] = new StandardServiceLevelService(),
        ["Express"] = new ExpresserviceLevelService()
    };

    public IEnumerable<string> AllowedOrderTypes =
    [
        typeof(PickupOrder).ToString(),
        typeof(DeliveryOrder).ToString()
    ];

    private readonly OrderValidationService OrderValidationService = new();
    private OrderCostingService? OrderCostingService;
    private readonly OrderReceiptService OrderReceiptService = new();
    private readonly ConsoleOrderNotifierService Notifier = new();
    private readonly OrderRepository OrderRepository = new();

    [Fact]
    public void Process_Success_ShouldRetrunStringMatches()
    {
        // Arrnage
        OrderCostingService = new(Policies, AllowedOrderTypes);
        
        var processor = new OrderProcessor(OrderValidationService, OrderCostingService, OrderReceiptService, Notifier, OrderRepository);
        var orderA = new DeliveryOrder("A-101", "Ava", 2, "Standard", "Metro");
        var orderB = new PickupOrder("A-102", "Noah", 1, "Express", "Store");

        var expectedOrderA = string.Join(Environment.NewLine,
            "Order A-101 for Ava",
            "Service: Standard",
            "Quantity: 2",
            "Subtotal: 50.00",
            "Fulfilment: 8.00",
            "Total: 58.00");

        var expectedOrderB = string.Join(Environment.NewLine,
        "Order A-102 for Noah",
        "Service: Express",
        "Quantity: 1",
        "Subtotal: 40.00",
        "Fulfilment: 0.00",
        "Total: 40.00");

        // Act
        var actualOrderA = processor.Process(orderA);
        var actualOrderB = processor.Process(orderB);

        // Assert
        Assert.Equal(expectedOrderA, actualOrderA);
        Assert.Equal(expectedOrderB, actualOrderB);
    }
}

public class OrderValidationServiceTests
{
    public static TheoryData<Order> InvalidOrders =
    [
        new DeliveryOrder("", "Ava", 2, "Standard", "Metro"),
        new DeliveryOrder("1", null!, 2, "Standard", "Metro"),
        new DeliveryOrder("1", "Ava", 0, "Standard", "Metro"),
        new DeliveryOrder("1", "Ava", 2, String.Empty, "Metro"),
    ];

    [Theory]
    [MemberData(nameof(InvalidOrders))]
    public void ValidateOrder_InvalidOrder_ShouldThrowArgumentException(Order order)
    {
        // Arrange
        var service = new OrderValidationService();
        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ValidateOrder(order));
    }

    [Fact]
    public void ValidateOrder_InvalidOrder_ShouldNotThrowArgumentException()
    {
        // Arrange
        var order = new DeliveryOrder("A-101", "Ava", 2, "Standard", "Metro");
        var service = new OrderValidationService();

        // Act
        service.ValidateOrder(order);
    }
}