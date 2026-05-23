using Catalog.API.Services;
using MediatR;

namespace Catalog.API.Application.Commands;

public class DeleteProductCommandHandler(
    IProductStore productStore,
    IProdutoCacheService cacheService) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var deleted = productStore.Delete(request.Id);

        if (!deleted)
            return false;

        await cacheService.InvalidateProductAsync(request.Id, cancellationToken);
        await cacheService.InvalidateProductsAsync(cancellationToken);

        return true;
    }
}
