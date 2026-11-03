using System.Reflection;
using System.Reflection.Metadata;
using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    private readonly List<IPurchase> Purchases =
    [
        new NewCustomerPurchase(),
        new InternationalPurchase(),
        new TotalPurchase(),
        new ItemPurchase()
    ];

    public void Run()
    {
        var purchases = new[]
        {
            new Purchase("P-101", 80m, 2, false, false),
            new Purchase("P-102", 650m, 1, true, true),
            new Purchase("P-103", 120m, 4, true, false),
            new Purchase("P-103", 20m, 10, false, false)
        };

        var assessor = new PurchaseRiskAssessor(Purchases);
        
        foreach (var purchase in purchases)
        {
            var result = assessor.Assess(purchase);
            Console.WriteLine($"{purchase.Id}: {result.Score} ({result.Level})");
        }

        // New requirement: orders with 10 or more items add 15 risk points.
        // Add an example demonstrating the new rule.
    }
}

public record Purchase(
    string Id,
    decimal Total,
    int ItemCount,
    bool International,
    bool NewCustomer);

public record RiskResult(int Score, string Level);

// OCP review: Can a new risk signal be added without editing the assessment
// algorithm or rewriting the existing signals?
// Real-world example: A fraud team introduces new detection signals often.
// Existing rules should remain stable while new rules are rolled out.

public interface IPurchaseRiskAssessor
{
    /// <summary>
    /// Assess the potential fraud for a purchase and return risk
    /// </summary>
    /// <param name="purchase"></param>
    /// <returns></returns>
    public RiskResult Assess(Purchase purchase);
}

public class PurchaseRiskAssessor(List<IPurchase> purchases) : IPurchaseRiskAssessor
{
    /// <inheritdoc />
    public RiskResult Assess(Purchase purchase)
    {
       ValidatePurchase(purchase);

        int score = 0;

        foreach(PropertyInfo property in purchase.GetType().GetProperties())
        {
            var purchaseType = purchases.FirstOrDefault(p => p.PurchaseType == property.Name);
            if(purchaseType != null)
                score += purchaseType.AddScore(purchase);
        }

        score = Math.Min(score, 100);

        var level = AssignRiskLevel(score);

        return new RiskResult(score, level);
    }

    /// <summary>
    /// Ensures a purchase has valid data
    /// </summary>
    /// <param name="purchase"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void ValidatePurchase(Purchase purchase)
    {
         if (string.IsNullOrWhiteSpace(purchase.Id) ||
            purchase.Total < 0m ||
            purchase.ItemCount <= 0)
        {
            throw new ArgumentException("Invalid purchase.");
        }
    }

    private static string AssignRiskLevel(int score)
    {
        return score switch
        {
            >= 60 => "High",
            >= 30 => "Medium",
            _ => "Low"
        };
    }
}

public interface IPurchase
{
    /// <summary>
    /// The type of purchase
    /// </summary>
    public string PurchaseType { get; }

    /// <summary>
    /// Adds risk score to a purchase
    /// </summary>
    /// <returns></returns>
    public int AddScore(Purchase purchase);
}

public class NewCustomerPurchase : IPurchase
{
    /// <inheritdoc />
    public string PurchaseType => "NewCustomer";

    /// <inheritdoc />
    public int AddScore(Purchase purchase) => purchase.NewCustomer ? 25 : 0;
}

public class InternationalPurchase : IPurchase
{
    /// <inheritdoc />
    public string PurchaseType => "International";

    /// <inheritdoc />
    public int AddScore(Purchase purchase) => purchase.International ? 20 : 0;
}

public class TotalPurchase : IPurchase
{
    /// <inheritdoc />
    public string PurchaseType => "Total";

    /// <inheritdoc />
    public int AddScore(Purchase purchase) => purchase.Total >= 500m ? 30 : 0;
}

public class ItemPurchase : IPurchase
{
    /// <inheritdoc />
    public string PurchaseType => "ItemCount";

    /// <inheritdoc />
    public int AddScore(Purchase purchase) => purchase.ItemCount >= 10 ? 15 : 0;
}