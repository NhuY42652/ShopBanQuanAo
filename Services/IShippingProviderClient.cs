using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public interface IShippingProviderClient
{
    Task<string> CreateShipmentAsync(Hoadon order, string address, CancellationToken cancellationToken = default);
}
