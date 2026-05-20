using GestaoPedidos.API.Models;
using GestaoPedidos.API.Clients;
using GestaoPedidos.Mensageria;
using MassTransit;

namespace GestaoPedidos.API.Services
{
    public class PedidoService
    {
        private readonly ICatalogClient _catalogClient;
        private readonly IPublishEndpoint _publishEndpoint;

        private static readonly List<Pedido> _pedidos = new();

        public PedidoService(ICatalogClient catalogClient, IPublishEndpoint publishEndpoint)
        {
            _catalogClient = catalogClient;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Pedido?> CriarPedido(Guid productId, int quantidade)
        {
            var produto = await _catalogClient.GetProductById(productId);

            if (produto == null)
                return null;

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantidade = quantidade,
                PrecoTotal = produto.Price * quantidade
            };

            _pedidos.Add(pedido);
            await _publishEndpoint.Publish(new PedidoCriado(
                pedido.Id,
                pedido.ProductId,
                pedido.Quantidade,
                pedido.PrecoTotal,
                DateTimeOffset.UtcNow));

            return pedido;
        }

        public List<Pedido> GetPedidos() => _pedidos;
    }
}
