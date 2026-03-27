namespace ShopBanQuanAoOnline.Services;

public class SmsGatewayClient : ISmsGatewayClient
{
    private readonly ILogger<SmsGatewayClient> _logger;

    public SmsGatewayClient(ILogger<SmsGatewayClient> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string phoneNumber, string content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SMS sent to {Phone}: {Content}", phoneNumber, content);
        return Task.CompletedTask;
    }
}
