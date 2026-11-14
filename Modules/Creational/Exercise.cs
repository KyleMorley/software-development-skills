using Interfaces;

namespace Modules.Creational;

public class Exercise : ILearningModule
{
    public string Name => "Factory Refactoring: Alert Delivery";

    public AlertFactory AlertFactory = new();
    public void Run()
    {
        var dispatcher = new AlertDispatcher(AlertFactory);
        
        var alerts = new[]
        {
            new AlertRequest("Email", "ops@example.test", "Database backup completed", false),
            new AlertRequest("Sms", "+61400000000", "Server temperature high", true),
            new AlertRequest("Push", "device-42", "New report available", false),
            new AlertRequest("Email", "oncall@example.test", "Payment gateway unavailable", true)
        };

        foreach (var alert in alerts)
            Console.WriteLine(dispatcher.Dispatch(alert));
    }
}

public record AlertRequest(string Channel, string Recipient, string Message, bool Urgent);

public interface IAlertSender
{
    string Send(string recipient, string message);
}

public sealed class EmailAlertSender(string fromAddress, bool highPriority) : IAlertSender
{
    public string Send(string recipient, string message) =>
        $"EMAIL from={fromAddress} to={recipient} priority={(highPriority ? "high" : "normal")}: {message}";
}

public sealed class SmsAlertSender(string senderId, bool urgent) : IAlertSender
{
    public string Send(string recipient, string message) =>
        $"SMS sender={senderId} to={recipient} urgent={urgent}: {message}";
}

public sealed class PushAlertSender(string applicationId, string urgency) : IAlertSender
{
    public string Send(string recipient, string message) =>
        $"PUSH app={applicationId} device={recipient} urgency={urgency}: {message}";
}

public interface IAlertFactory
{
    /// <summary>
    /// Creates an alert
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public IAlertSender CreateAlert(AlertRequest request);
}

public class AlertFactory : IAlertFactory
{
    /// <inheritdoc />
    public IAlertSender CreateAlert(AlertRequest request)
    {
        IAlertSender sender;
        if (request.Channel.Equals("Email", StringComparison.OrdinalIgnoreCase))
            sender = new EmailAlertSender("alerts@example.test", request.Urgent);
        else if (request.Channel.Equals("Sms", StringComparison.OrdinalIgnoreCase))
            sender = new SmsAlertSender("SystemAlerts", request.Urgent);
        else if (request.Channel.Equals("Push", StringComparison.OrdinalIgnoreCase))
            sender = new PushAlertSender("operations-app", request.Urgent ? "critical" : "standard");
        else
            throw new ArgumentException($"Unsupported alert channel: {request.Channel}", nameof(request));

        return sender;
    }
}

public sealed class AlertDispatcher(IAlertFactory alertFactory)
{
    public string Dispatch(AlertRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sender = alertFactory.CreateAlert(request);

        return sender.Send(request.Recipient, request.Message);
    }
}
