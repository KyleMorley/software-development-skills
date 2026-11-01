using Interfaces;

ILearningModule[] modules =
[
    // Add instances of classes that implement ILearningModule here
];


while (true)
{
    foreach (var module in modules)
    {
        module.Run();
    }
}