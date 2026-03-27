namespace ShopBanQuanAoOnline.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyOrderStatusChangedAsync(int orderId, string newStatus, string recipient, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Order {OrderId} changed to {Status}. Notification recipient: {Recipient}",
            orderId,
            newStatus,
            recipient);

        return Task.CompletedTask;
    }
}
