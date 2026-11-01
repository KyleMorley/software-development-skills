using Interfaces;

ILearningModule[] modules =
[
    // Register an exercise here, for example:
    // new Modules.SOLID.SRP.Exercise()
];

if (modules.Length == 0)
{
    Console.WriteLine("No exercises registered. Read exercises/current/brief.md to get started.");
    return;
}

foreach (ILearningModule module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
