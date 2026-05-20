using System.Net;
using System.Net.Http.Json;
using GestaoPedidos.API.Clients;
using GestaoPedidos.API.Models;
using Moq;

namespace gestaopedidos.test;

public sealed class PedidosEndpointTests : IClassFixture<RabbitMqFixture>
{
    private readonly RabbitMqFixture _rabbitMq;

    public PedidosEndpointTests(RabbitMqFixture rabbitMq)
    {
        _rabbitMq = rabbitMq;
    }

    [Fact]
    public async Task PostApiPedidos_DeveCriarPedidoERetornarOk()
    {
        var productId = Guid.NewGuid();
        var catalogClientMock = new Mock<ICatalogClient>();
        catalogClientMock
            .Setup(client => client.GetProductById(productId))
            .ReturnsAsync(new ProductDto
            {
                Id = productId,
                Name = "Livro de teste",
                Price = 25m
            });

        await using var factory = new PedidosApiFactory(_rabbitMq, catalogClientMock);
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync("/api/pedidos", new CriarPedidoRequest
        {
            ProductId = productId,
            Quantidade = 2
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pedido = await response.Content.ReadFromJsonAsync<Pedido>();

        Assert.NotNull(pedido);
        Assert.Equal(productId, pedido!.ProductId);
        Assert.Equal(2, pedido.Quantidade);
        Assert.Equal(50m, pedido.PrecoTotal);

        catalogClientMock.Verify(client => client.GetProductById(productId), Times.Once);
    }
}
