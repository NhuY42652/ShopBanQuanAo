namespace ShopBanQuanAoOnline.Services;

public interface ISmsGatewayClient
{
    Task SendAsync(string phoneNumber, string content, CancellationToken cancellationToken = default);
}
