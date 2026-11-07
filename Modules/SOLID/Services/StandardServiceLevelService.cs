using Modules.SOLID.Interfaces;

namespace Modules.SOLID.Services;

public class StandardServiceLevelService : IServiceLevelPolicy
{
    /// <inheritdoc />
    public string PolicyName => "Standard";

    /// <inheritdoc />
    public decimal ApplyFee()
    {
        return 25m;
    }
}