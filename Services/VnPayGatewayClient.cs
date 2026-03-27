using System.Globalization;
using System.Security.Cryptography;
using System.Text;
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
        var hashSecret = _configuration["ExternalServices:VnPay:HashSecret"];

        var createDate = DateTime.UtcNow.AddHours(7); // VN timezone
        var expireDate = createDate.AddMinutes(15);
        var txnRef = $"HD{order.MaHd}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var vnPayParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = ((long)(amount * 100)).ToString(CultureInfo.InvariantCulture),
            ["vnp_CreateDate"] = createDate.ToString("yyyyMMddHHmmss"),
            ["vnp_CurrCode"] = "VND",
            ["vnp_IpAddr"] = "127.0.0.1",
            ["vnp_Locale"] = "vn",
            ["vnp_OrderInfo"] = $"Thanh toan don hang {order.MaHd}",
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_TxnRef"] = txnRef,
            ["vnp_ExpireDate"] = expireDate.ToString("yyyyMMddHHmmss")
        };

        var queryData = BuildQueryData(vnPayParams);
        if (!string.IsNullOrWhiteSpace(hashSecret))
        {
            var secureHash = ComputeHmacSha512(hashSecret, queryData);
            vnPayParams["vnp_SecureHash"] = secureHash;
        }

        var paymentUrl = $"{baseUrl}?{BuildQueryData(vnPayParams)}";
        _logger.LogInformation("Generated VNPay payment url for order {OrderId}", order.MaHd);
        return Task.FromResult(paymentUrl);
    }

    private static string BuildQueryData(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        return string.Join("&", parameters
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }

    private static string ComputeHmacSha512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}