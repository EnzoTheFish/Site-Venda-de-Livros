using Catalog.API.Models;

namespace Catalog.API.Services;

public interface IProductStore
{
    IReadOnlyCollection<Product> GetAll();
    Product? GetById(Guid id);
    Product Add(Product product);
    bool Update(Guid id, Product product);
    bool Delete(Guid id);
}
