namespace ShopBanQuanAoOnline.Services;

public interface INotificationService
{
    Task NotifyOrderStatusChangedAsync(int orderId, string newStatus, string recipient, CancellationToken cancellationToken = default);
}
