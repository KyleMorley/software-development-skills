using Interfaces;

ILearningModule[] modules =
[
    new Modules.Factory.Exercise()
];

foreach (var module in modules)
{
    Console.WriteLine($"--- {module.Name} ---");
    module.Run();
}
