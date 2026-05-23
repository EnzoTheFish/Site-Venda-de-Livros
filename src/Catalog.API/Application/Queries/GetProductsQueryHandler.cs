using Catalog.API.Models;
using Catalog.API.Services;
using MediatR;

namespace Catalog.API.Application.Queries;

public class GetProductsQueryHandler(
    IProductStore productStore,
    IProdutoCacheService cacheService) : IRequestHandler<GetProductsQuery, IReadOnlyCollection<Product>>
{
    public async Task<IReadOnlyCollection<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cachedProducts = await cacheService.GetProductsAsync(cancellationToken);

        if (cachedProducts != null)
            return cachedProducts;

        var products = productStore.GetAll();
        await cacheService.SetProductsAsync(products, cancellationToken);

        return products;
    }
}
