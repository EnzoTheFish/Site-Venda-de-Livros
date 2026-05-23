using System.Net;
using System.Net.Http.Json;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace GestaoPedidos.Tests;

public sealed class CatalogCacheAsideTests : IClassFixture<RedisFixture>
{
    private const string RedisInstanceName = "catalog-test:";
    private static readonly Guid SeedProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly RedisFixture _redis;

    public CatalogCacheAsideTests(RedisFixture redis)
    {
        _redis = redis;
    }

    [Fact]
    public async Task GetProductById_DeveUsarCacheAsideEInvalidarNoUpdate()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Redis"] = _redis.ConnectionString,
                        ["Redis:InstanceName"] = RedisInstanceName
                    });
                });
            });

        using var client = factory.CreateClient();
        await using var redis = await ConnectionMultiplexer.ConnectAsync(_redis.ConnectionString);
        var database = redis.GetDatabase();
        var productCacheKey = $"{RedisInstanceName}products:{SeedProductId}";
        var productsCacheKey = $"{RedisInstanceName}products:all";

        await database.KeyDeleteAsync(productCacheKey);
        await database.KeyDeleteAsync(productsCacheKey);

        var firstResponse = await client.GetAsync($"/api/v1/products/{SeedProductId}");

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.True(await database.KeyExistsAsync(productCacheKey));

        var secondProduct = await client.GetFromJsonAsync<Product>($"/api/v1/products/{SeedProductId}");

        Assert.NotNull(secondProduct);
        Assert.Equal("LivroTeste1", secondProduct!.Name);

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/products/{SeedProductId}", new Product
        {
            Name = "Livro cache atualizado",
            Description = "Produto atualizado para validar invalidacao explicita",
            Price = 31.90m,
            Stock = 7
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.False(await database.KeyExistsAsync(productCacheKey));
        Assert.False(await database.KeyExistsAsync(productsCacheKey));

        var updatedProduct = await client.GetFromJsonAsync<Product>($"/api/v1/products/{SeedProductId}");

        Assert.NotNull(updatedProduct);
        Assert.Equal("Livro cache atualizado", updatedProduct!.Name);
        Assert.True(await database.KeyExistsAsync(productCacheKey));
    }
}
