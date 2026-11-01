using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        Dictionary<string, INotificationService> services = new()
        {
            ["Email"] = new EmailService(),
            ["Sms"] = new SMSService(),
            ["Push"] = new PushService(),
            ["WebhookService"] = new WebhookService()
        };

        var message = new Notification(
            "alex@example.com",
            "Your order has shipped",
            "Order A-104 is on its way.");

        foreach (var channel in new[] { "Email", "Sms", "Push", "WebhookService" })
        {
            services.TryGetValue(channel, out var service);
            if(service is null)
                throw new Exception("Non supported service.");

            Console.WriteLine(service.Notify(message));
        }
    }
}

public record Notification(string Recipient, string Subject, string Body);

public interface INotificationService
{
    /// <summary>
    /// Send push notification
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    public string Notify(Notification notification);
}

public interface IEmailService : INotificationService
{  
}

public interface ISMSService : INotificationService
{
}

public interface IPushService : INotificationService
{
}

public interface IWebhookService : INotificationService
{
}

public class EmailService : IEmailService
{
    /// <inheritdoc />
    public string Notify(Notification notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Recipient))
            throw new ArgumentException("Recipient is required.");

        return $"EMAIL TO {notification.Recipient}: {notification.Subject} - {notification.Body}";
    }
}
public class SMSService : ISMSService
{
    /// <inheritdoc />
    public string Notify(Notification notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Recipient))
            throw new ArgumentException("Recipient is required.");

        return $"SMS TO {notification.Recipient}: {CreateSmsBody(notification)}";
    }

    /// <summary>
    /// Creates sms body
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    private static string CreateSmsBody(Notification notification)
    {
        return notification.Body.Length > 20
                    ? notification.Body[..20] + "..."
                    : notification.Body;
    }
}

public class PushService : IPushService
{
    /// <inheritdoc />
    public string Notify(Notification notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Recipient))
            throw new ArgumentException("Recipient is required.");

        return $"PUSH TO {notification.Recipient}: {notification.Subject}";
    }   
}

public class WebhookService : IWebhookService
{
    /// <inheritdoc />
    public string Notify(Notification notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Recipient))
            throw new ArgumentException("Recipient is required.");

        return $"WBHOOK|{notification.Recipient}|{notification.Subject}|{notification.Body}";
    }   
}