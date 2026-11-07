using Interfaces;

ILearningModule[] modules =
[
    new Modules.SOLID.OCP.Exercise()
];

foreach (var module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
