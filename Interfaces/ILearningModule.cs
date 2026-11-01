namespace Interfaces;

public interface ILearningModule
{
    /// <summary>
    /// Gets the name of the learning module.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Runs the learning module.
    /// </summary>
    public void Run();
}