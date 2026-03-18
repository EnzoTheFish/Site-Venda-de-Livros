using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new();

        // LISTAR TODOS
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return Ok(_products);
        }

        // BUSCAR
        [HttpGet("{id}")]
        public ActionResult<Product> GetById(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // CRIAR
        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            product.Id = Guid.NewGuid();
            _products.Add(product);

            return CreatedAtAction(nameof(GetById),
                new { id = product.Id }, product);
        }

        // ATUALIZAR
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Product updatedProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;

            return NoContent();
        }

        // DELETAR
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            _products.Remove(product);

            return NoContent();
        }
    }
}