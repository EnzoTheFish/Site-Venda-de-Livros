using GestaoPedidos.API.Clients;
using GestaoPedidos.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace gestaopedidos.test;

public sealed class PedidosApiFactory(RabbitMqFixture rabbitMq, Mock<ICatalogClient> catalogClientMock)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMq:Host"] = rabbitMq.Host,
                ["RabbitMq:Port"] = rabbitMq.Port.ToString(),
                ["RabbitMq:Username"] = RabbitMqFixture.Username,
                ["RabbitMq:Password"] = RabbitMqFixture.Password
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICatalogClient>();
            services.AddSingleton(catalogClientMock.Object);
        });
    }
}
