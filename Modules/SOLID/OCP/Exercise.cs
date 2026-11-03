using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        var purchases = new[]
        {
            new Purchase("P-101", 80m, 2, false, false),
            new Purchase("P-102", 650m, 1, true, true),
            new Purchase("P-103", 120m, 4, true, false)
        };

        var assessor = new PurchaseRiskAssessor();
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
public class PurchaseRiskAssessor
{
    public RiskResult Assess(Purchase purchase)
    {
        if (string.IsNullOrWhiteSpace(purchase.Id) ||
            purchase.Total < 0m ||
            purchase.ItemCount <= 0)
        {
            throw new ArgumentException("Invalid purchase.");
        }

        int score = 0;

        if (purchase.NewCustomer)
            score += 25;

        if (purchase.International)
            score += 20;

        if (purchase.Total >= 500m)
            score += 30;

        score = Math.Min(score, 100);

        string level = score switch
        {
            >= 60 => "High",
            >= 30 => "Medium",
            _ => "Low"
        };

        return new RiskResult(score, level);
    }
}
