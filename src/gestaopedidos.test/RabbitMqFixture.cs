using Testcontainers.RabbitMq;

namespace gestaopedidos.test;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    public const string Username = "gestaopedidos";
    public const string Password = "gestaopedidos";

    private readonly RabbitMqContainer _container = new RabbitMqBuilder("rabbitmq:3-management")
        .WithUsername(Username)
        .WithPassword(Password)
        .Build();

    public string Host => _container.Hostname;
    public ushort Port => _container.GetMappedPublicPort(5672);

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
