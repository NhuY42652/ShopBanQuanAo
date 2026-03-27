using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public interface IRecommendationService
{
    Task<IReadOnlyList<Mathang>> GetRecommendedProductsAsync(int customerId, int take = 6, CancellationToken cancellationToken = default);
}
