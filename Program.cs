using Interfaces;

ILearningModule[] modules =
[
    new Modules.Linq.Exercise()
];

foreach (var module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
