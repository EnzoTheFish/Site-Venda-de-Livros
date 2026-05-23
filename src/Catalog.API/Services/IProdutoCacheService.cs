using Catalog.API.Models;

namespace Catalog.API.Services;

public interface IProdutoCacheService
{
    Task<IReadOnlyCollection<Product>?> GetProductsAsync(CancellationToken cancellationToken);
    Task SetProductsAsync(IReadOnlyCollection<Product> products, CancellationToken cancellationToken);
    Task<Product?> GetProductAsync(Guid id, CancellationToken cancellationToken);
    Task SetProductAsync(Product product, CancellationToken cancellationToken);
    Task InvalidateProductAsync(Guid id, CancellationToken cancellationToken);
    Task InvalidateProductsAsync(CancellationToken cancellationToken);
}
