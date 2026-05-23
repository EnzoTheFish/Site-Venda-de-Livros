using Catalog.API.Models;
using Catalog.API.Services;
using MediatR;

namespace Catalog.API.Application.Queries;

public class GetProductByIdQueryHandler(
    IProductStore productStore,
    IProdutoCacheService cacheService) : IRequestHandler<GetProductByIdQuery, Product?>
{
    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cachedProduct = await cacheService.GetProductAsync(request.Id, cancellationToken);

        if (cachedProduct != null)
            return cachedProduct;

        var product = productStore.GetById(request.Id);

        if (product != null)
            await cacheService.SetProductAsync(product, cancellationToken);

        return product;
    }
}
