using Interfaces;

namespace Modules.SOLID.OCP;

public class Exercise : ILearningModule
{
    public string Name => "Open/Closed Principle";

    public void Run()
    {
        var message = new Notification(
            "alex@example.com",
            "Your order has shipped",
            "Order A-104 is on its way.");

        var sender = new NotificationSender();
        foreach (var channel in new[] { "Email", "Sms", "Push" })
        {
            Console.WriteLine(sender.Send(channel, message));
        }

        // New requirement: support a "Webhook" channel using the same Send operation.
        // Format: WEBHOOK|recipient|subject|body
        // Existing channels and unknown-channel behaviour must remain unchanged.
    }
}

public record Notification(string Recipient, string Subject, string Body);

// OCP review: What happens here whenever the product supports another channel?
// How could a new channel be introduced without modifying existing channel rules?
public class NotificationSender
{
    public string Send(string channel, Notification notification)
    {
        if (string.IsNullOrWhiteSpace(notification.Recipient))
            throw new ArgumentException("Recipient is required.");

        switch (channel)
        {
            case "Email":
                return $"EMAIL TO {notification.Recipient}: {notification.Subject} - {notification.Body}";

            case "Sms":
                var smsBody = notification.Body.Length > 20
                    ? notification.Body[..20] + "..."
                    : notification.Body;
                return $"SMS TO {notification.Recipient}: {smsBody}";

            case "Push":
                return $"PUSH TO {notification.Recipient}: {notification.Subject}";

            default:
                throw new NotSupportedException($"Unsupported channel: {channel}");
        }
    }
}
