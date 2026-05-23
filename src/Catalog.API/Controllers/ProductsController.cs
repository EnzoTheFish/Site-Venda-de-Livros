using Catalog.API.Application.Commands;
using Catalog.API.Application.Queries;
using Catalog.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(CancellationToken cancellationToken)
        {
            var products = await _sender.Send(new GetProductsQuery(), cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product, CancellationToken cancellationToken)
        {
            var createdProduct = await _sender.Send(new CreateProductCommand(product), cancellationToken);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product updatedProduct, CancellationToken cancellationToken)
        {
            var updated = await _sender.Send(new UpdateProductCommand(id, updatedProduct), cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _sender.Send(new DeleteProductCommand(id), cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
