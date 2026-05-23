using Catalog.API.Models;
using Catalog.API.Services;
using MediatR;

namespace Catalog.API.Application.Commands;

public class CreateProductCommandHandler(
    IProductStore productStore,
    IProdutoCacheService cacheService) : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = productStore.Add(request.Product);
        await cacheService.InvalidateProductsAsync(cancellationToken);
        await cacheService.SetProductAsync(product, cancellationToken);

        return product;
    }
}
