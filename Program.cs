using Interfaces;

ILearningModule[] modules =
[
    new Modules.SOLID.SRP.Exercise()
];

foreach (ILearningModule module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
