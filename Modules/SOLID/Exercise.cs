using Interfaces;
using Modules.SOLID.Interfaces;
using Modules.SOLID.Models;
using Modules.SOLID.Repositories;
using Modules.SOLID.Services;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public IReadOnlyDictionary<string, IServiceLevelPolicy> Policies = new
        Dictionary<string, IServiceLevelPolicy>
    {
        ["Standard"] = new StandardServiceLevelService(),
        ["Express"] = new ExpresserviceLevelService()
    };

    public IEnumerable<string> AllowedOrderTypes =
    [
        typeof(PickupOrder).Name,
        typeof(DeliveryOrder).Name
    ];

    public string Name => "SOLID Refactoring: Order Fulfilment";
    private readonly OrderValidationService OrderValidationService = new();
    private OrderCostingService? OrderCostingService;
    private readonly OrderReceiptService OrderReceiptService = new();
    private readonly ConsoleOrderNotifierService Notifier = new();
    private readonly OrderRepository OrderRepository = new();

    public void Run()
    {
        OrderCostingService = new(Policies, AllowedOrderTypes);

        var processor = new OrderProcessor(OrderValidationService, OrderCostingService, OrderReceiptService, Notifier, OrderRepository);
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

        foreach (var record in OrderRepository.Records)
            Console.WriteLine(record);
    }
}

public class OrderProcessor(IOrderValidationService orderValidationService,
    IOrderCostingService orderCostingService,
    IOrderReceiptService orderReceiptService,
    IConsoleOrderNotifier consoleOrderNotifier,
    IOrderRepository orderRepository)
{
    public string Process(Order order)
    {
        orderValidationService.ValidateOrder(order);

        var costs = orderCostingService.CalculateCosts(order);
        var receipt = orderReceiptService.GenerateReceipt(order, costs);

        orderRepository.SaveOrder(order.Id, costs.Total);
        consoleOrderNotifier.Notify(order.Customer, order.Id);

        return receipt;
    }
}