using GestaoPedidos.API.Models;
using GestaoPedidos.API.Clients;

namespace GestaoPedidos.API.Services
{
    public class PedidoService
    {
        private readonly ICatalogClient _catalogClient;

        private static readonly List<Pedido> _pedidos = new();

        public PedidoService(ICatalogClient catalogClient)
        {
            _catalogClient = catalogClient;
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

            return pedido;
        }

        public List<Pedido> GetPedidos() => _pedidos;
    }
}