using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public interface IPaymentGatewayClient
{
    Task<string> CreatePaymentUrlAsync(Hoadon order, decimal amount, string returnUrl, CancellationToken cancellationToken = default);
}
