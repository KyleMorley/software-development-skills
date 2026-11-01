using Interfaces;

namespace Modules.SOLID.SRP;

/// <summary>
/// Provides a starting point for a Single Responsibility Principle exercise.
/// </summary>
public class Exercise : ILearningModule
{
    /// <inheritdoc />
    public string Name => "Single Responsibility Principle";

    /// <inheritdoc />
    public void Run()
    {
        Console.WriteLine("No SRP exercise has been generated yet.");
    }
}
