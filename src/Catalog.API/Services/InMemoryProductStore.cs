using Catalog.API.Models;

namespace Catalog.API.Services;

public class InMemoryProductStore : IProductStore
{
    private readonly List<Product> _products =
    [
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "LivroTeste1", Description = "Um Livro Legal", Price = 20.00M, Stock = 10 },
        new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "LivroDiferente2", Description = "Um Livro Diferente", Price = 25.00M, Stock = 50 }
    ];

    public IReadOnlyCollection<Product> GetAll()
    {
        lock (_products)
            return _products.Select(Clone).ToList();
    }

    public Product? GetById(Guid id)
    {
        lock (_products)
            return _products.Where(product => product.Id == id).Select(Clone).FirstOrDefault();
    }

    public Product Add(Product product)
    {
        var created = Clone(product);
        created.Id = Guid.NewGuid();

        lock (_products)
            _products.Add(created);

        return Clone(created);
    }

    public bool Update(Guid id, Product product)
    {
        lock (_products)
        {
            var existingProduct = _products.FirstOrDefault(item => item.Id == id);

            if (existingProduct == null)
                return false;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            return true;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_products)
        {
            var product = _products.FirstOrDefault(item => item.Id == id);

            if (product == null)
                return false;

            _products.Remove(product);
            return true;
        }
    }

    private static Product Clone(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock
    };
}
