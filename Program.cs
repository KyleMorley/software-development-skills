using Interfaces;

ILearningModule[] modules =
[
    // Register an exercise here, for example:
    // new Modules.SOLID.SRP.Exercise()
];

if (modules.Length == 0)
{
    Console.WriteLine("No exercises registered. Ask your coach for an exercise.");
    return;
}

foreach (ILearningModule module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
