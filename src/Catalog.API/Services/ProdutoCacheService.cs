using System.Diagnostics;
using System.Text.Json;
using Catalog.API.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.API.Services;

public class ProdutoCacheService(
    IDistributedCache cache,
    ILogger<ProdutoCacheService> logger) : IProdutoCacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        SlidingExpiration = TimeSpan.FromMinutes(2)
    };

    public async Task<IReadOnlyCollection<Product>?> GetProductsAsync(CancellationToken cancellationToken)
    {
        var startedAt = Stopwatch.GetTimestamp();
        var cached = await cache.GetStringAsync(CacheKeys.Products, cancellationToken);

        if (cached == null)
        {
            LogCache("MISS", CacheKeys.Products, startedAt);
            return null;
        }

        LogCache("HIT", CacheKeys.Products, startedAt);
        return JsonSerializer.Deserialize<List<Product>>(cached, JsonOptions);
    }

    public async Task SetProductsAsync(IReadOnlyCollection<Product> products, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync(
            CacheKeys.Products,
            JsonSerializer.Serialize(products, JsonOptions),
            CacheOptions,
            cancellationToken);

        logger.LogInformation("Redis SET {CacheKey} com {Count} produtos.", CacheKeys.Products, products.Count);
    }

    public async Task<Product?> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Product(id);
        var startedAt = Stopwatch.GetTimestamp();
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached == null)
        {
            LogCache("MISS", cacheKey, startedAt);
            return null;
        }

        LogCache("HIT", cacheKey, startedAt);
        return JsonSerializer.Deserialize<Product>(cached, JsonOptions);
    }

    public async Task SetProductAsync(Product product, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Product(product.Id);

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product, JsonOptions),
            CacheOptions,
            cancellationToken);

        logger.LogInformation("Redis SET {CacheKey}.", cacheKey);
    }

    public async Task InvalidateProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Product(id);
        await cache.RemoveAsync(cacheKey, cancellationToken);
        logger.LogInformation("Redis DEL {CacheKey}.", cacheKey);
    }

    public async Task InvalidateProductsAsync(CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(CacheKeys.Products, cancellationToken);
        logger.LogInformation("Redis DEL {CacheKey}.", CacheKeys.Products);
    }

    private void LogCache(string status, string cacheKey, long startedAt)
    {
        var elapsed = Stopwatch.GetElapsedTime(startedAt);
        logger.LogInformation("Redis {Status} {CacheKey} em {ElapsedMilliseconds}ms.", status, cacheKey, elapsed.TotalMilliseconds);
    }

    private static class CacheKeys
    {
        public const string Products = "products:all";

        public static string Product(Guid id) => $"products:{id}";
    }
}
