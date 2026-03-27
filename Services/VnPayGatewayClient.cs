using System.Web;
using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public class VnPayGatewayClient : IPaymentGatewayClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<VnPayGatewayClient> _logger;

    public VnPayGatewayClient(IConfiguration configuration, ILogger<VnPayGatewayClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<string> CreatePaymentUrlAsync(Hoadon order, decimal amount, string returnUrl, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["ExternalServices:VnPay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        var tmnCode = _configuration["ExternalServices:VnPay:TmnCode"] ?? "DEMO";

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["vnp_TmnCode"] = tmnCode;
        query["vnp_Amount"] = ((long)(amount * 100)).ToString();
        query["vnp_OrderInfo"] = $"Thanh toan don hang {order.MaHd}";
        query["vnp_ReturnUrl"] = returnUrl;
        query["vnp_TxnRef"] = $"HD{order.MaHd}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var paymentUrl = $"{baseUrl}?{query}";
        _logger.LogInformation("Generated VNPay payment url for order {OrderId}", order.MaHd);
        return Task.FromResult(paymentUrl);
    }
}
