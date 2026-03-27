using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public class GhnShippingClient : IShippingProviderClient
{
    private readonly ILogger<GhnShippingClient> _logger;

    public GhnShippingClient(ILogger<GhnShippingClient> logger)
    {
        _logger = logger;
    }

    public Task<string> CreateShipmentAsync(Hoadon order, string address, CancellationToken cancellationToken = default)
    {
        var shipmentCode = $"GHN-{order.MaHd}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        _logger.LogInformation("Created GHN shipment {ShipmentCode} for order {OrderId} to {Address}", shipmentCode, order.MaHd, address);
        return Task.FromResult(shipmentCode);
    }
}
