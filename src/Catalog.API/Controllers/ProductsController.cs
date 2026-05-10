using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;
namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // Versionamento por URI
    public class ProductsController : ControllerBase {
    private static readonly List<Product> _products = new List<Product> {
            new Product { Id = Guid.NewGuid(), Name = "LivroTeste1", Description =  "Um Livro Legal", Price = 20.00M, Stock = 10 },
            new Product { Id = Guid.NewGuid(), Name = "LivroDiferente2", Description = "Um Livro Diferente", Price = 25.00M, Stock = 50 }
    };
    [HttpGet]
         public ActionResult<IEnumerable<Product>> GetProducts() {
               return Ok(_products);
             }
    [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(Guid id){ var product = _products.FirstOrDefault(p => p.Id == id);
                    if (product == null) {
                    return NotFound(); // Retorna 404 Not Found
                    }
                    return Ok(product); }
        [HttpPost]
    public ActionResult<Product> CreateProduct(Product product) { product.Id = Guid.NewGuid(); _products.Add(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id}, product); // Retorna 201 Created
    }
        [HttpPut("{id}")]
          public IActionResult UpdateProduct(Guid id, Product updatedProduct) {
                var existingProduct = _products.FirstOrDefault(p => p.Id == id);
                    if (existingProduct == null) {
                            return NotFound(); 
                            }
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Stock = updatedProduct.Stock;
                return NoContent(); // Retorna 204 No Content
            }
    [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id) {
            var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null) {
                    return NotFound();
                    }
            _products.Remove(product);
            return NoContent(); // Retorna 204 No Content
            }
    }
}