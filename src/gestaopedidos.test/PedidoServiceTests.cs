using GestaoPedidos.API.Clients;
using GestaoPedidos.API.Models;
using GestaoPedidos.API.Services;
using GestaoPedidos.Mensageria;
using MassTransit;
using Moq;

namespace gestaopedidos.test;

public class PedidoServiceTests
{
    [Fact]
    public async Task CriarPedido_DeveCalcularTotalEPublicarEvento()
    {
        var productId = Guid.NewGuid();
        var catalogClientMock = new Mock<ICatalogClient>();
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        catalogClientMock
            .Setup(client => client.GetProductById(productId))
            .ReturnsAsync(new ProductDto
            {
                Id = productId,
                Name = "Livro de teste",
                Price = 30m
            });

        var service = new PedidoService(catalogClientMock.Object, publishEndpointMock.Object);

        var pedido = await service.CriarPedido(productId, 3);

        Assert.NotNull(pedido);
        Assert.Equal(productId, pedido!.ProductId);
        Assert.Equal(3, pedido.Quantidade);
        Assert.Equal(90m, pedido.PrecoTotal);

        publishEndpointMock.Verify(endpoint => endpoint.Publish(
            It.Is<PedidoCriado>(message =>
                message.PedidoId == pedido.Id &&
                message.ProductId == productId &&
                message.Quantidade == 3 &&
                message.PrecoTotal == 90m),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
