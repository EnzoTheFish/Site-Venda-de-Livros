using Microsoft.AspNetCore.Mvc;
using GestaoPedidos.API.Services;
using GestaoPedidos.API.Models;

namespace GestaoPedidos.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _service;

        public PedidosController(PedidoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> Criar(Guid productId, int quantidade)
        {
            var pedido = await _service.CriarPedido(productId, quantidade);

            if (pedido == null)
                return NotFound("Produto não encontrado");

            return Ok(pedido);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pedido>> Get()
        {
            return Ok(_service.GetPedidos());
        }
    }
}