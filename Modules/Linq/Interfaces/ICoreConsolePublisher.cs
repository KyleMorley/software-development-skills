using Modules.Linq.Models;

namespace Modules.Linq.Interfaces;

public interface ICoreConsolePublisher
{
    /// <summary>
    /// Publishes alerts
    /// </summary>
    /// <param name="alerts"></param>
    public void Publish(List<SensorAlert> alerts);
}