using Modules.SOLID.Models;

namespace Modules.SOLID.Interfaces;

public interface IServiceLevelPolicy
{
    /// <summary>
    /// Name of the service level policy
    /// </summary>
    public string PolicyName { get; }

    /// <summary>
    /// Apply the service level policy fee.
    /// </summary>
    /// <returns></returns>
    public decimal ApplyFee();
}