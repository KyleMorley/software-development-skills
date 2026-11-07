using Modules.SOLID.Interfaces;

namespace Modules.SOLID.Services;

public class ExpresserviceLevelService : IServiceLevelPolicy
{
    /// <inheritdoc />
    public string PolicyName => "Express";

    /// <inheritdoc />
    public decimal ApplyFee()
    {
        return 40m;
    }
}