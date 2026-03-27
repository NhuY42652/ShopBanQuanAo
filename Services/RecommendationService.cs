using Microsoft.EntityFrameworkCore;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ApplicationDbContext _dbContext;

    public RecommendationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Mathang>> GetRecommendedProductsAsync(int customerId, int take = 6, CancellationToken cancellationToken = default)
    {
        if (take <= 0)
        {
            return Array.Empty<Mathang>();
        }

        var preferredCategoryIds = await _dbContext.Hoadons
            .Where(h => h.MaKh == customerId)
            .SelectMany(h => h.Cthoadons)
            .Select(c => c.MaMhNavigation.MaDm)
            .GroupBy(dm => dm)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);

        if (preferredCategoryIds.Count == 0)
        {
            return await _dbContext.Mathangs
                .OrderByDescending(m => m.LuotMua)
                .ThenByDescending(m => m.LuotXem)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        return await _dbContext.Mathangs
            .Where(m => preferredCategoryIds.Contains(m.MaDm))
            .OrderByDescending(m => m.LuotMua)
            .ThenByDescending(m => m.LuotXem)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
