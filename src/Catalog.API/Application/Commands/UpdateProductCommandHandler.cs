using Catalog.API.Services;
using MediatR;

namespace Catalog.API.Application.Commands;

public class UpdateProductCommandHandler(
    IProductStore productStore,
    IProdutoCacheService cacheService) : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var updated = productStore.Update(request.Id, request.Product);

        if (!updated)
            return false;

        await cacheService.InvalidateProductAsync(request.Id, cancellationToken);
        await cacheService.InvalidateProductsAsync(cancellationToken);

        return true;
    }
}
