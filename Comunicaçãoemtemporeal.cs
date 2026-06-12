namespace GestaoPedidos.Domain.Events;

public record PedidoStatusAlteradoEvent(
    Guid PedidoId,
    string StatusAnterior,
    string NovoStatus,
    DateTime AlteradoEm,
    string? Observacao
);
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GestaoPedidos.API.Hubs;

[Authorize] // Autenticação JWT exigida para conexões SignalR
public class PedidoHub : Hub
{
    public static string GrupoKey(Guid pedidoId) => $"pedido:{pedidoId}";

    public async Task AssinarPedido(Guid pedidoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoKey(pedidoId));
    }

    public async Task CancelarAssinaturaPedido(Guid pedidoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GrupoKey(pedidoId));
    }
}

using GestaoPedidos.API.Hubs;
using GestaoPedidos.Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace GestaoPedidos.Infrastructure.Consumers;

public class PedidoStatusAlteradoConsumer : IConsumer<PedidoStatusAlteradoEvent>
{
    private readonly IHubContext<PedidoHub> _hubContext;

    public PedidoStatusAlteradoConsumer(IHubContext<PedidoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<PedidoStatusAlteradoEvent> context)
    {
        var evento = context.Message;
        
        // Notifica apenas os clientes que estão no grupo específico deste pedido
        await _hubContext.Clients
            .Group(PedidoHub.GrupoKey(evento.PedidoId))
            .SendAsync("StatusAtualizado", new {
                evento.PedidoId,
                evento.StatusAnterior,
                Status = evento.NovoStatus,
                evento.AlteradoEm,
                evento.Observacao
            });
    }
}

[cite_start]// 1. Registro de serviços [cite: 75]
builder.Services.AddSignalR().AddStackExchangeRedis(
    builder.Configuration.GetConnectionString("Redis")!,
    opts => opts.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("GestaoPedidos:signalr:")
); // Configuração do Redis Backplane para múltiplas instâncias [cite: 76, 77, 78]

builder.Services.AddCors(o => o.AddPolicy("SignalRPolicy", p =>
    p.WithOrigins("http://localhost:4200")
     .AllowAnyHeader()
     .AllowAnyMethod()
     [cite_start].AllowCredentials() // Obrigatório para WebSocket com autenticação [cite: 73, 81]
));

// ... outros serviços ...

var app = builder.Build();

[cite_start]// 2. Pipeline HTTP (ORDEM CRÍTICA) [cite: 82]
app.UseCors("SignalRPolicy"); // DEVE vir antes de UseAuthentication [cite: 83, 89]
[cite_start]app.UseAuthentication(); [cite: 84]
[cite_start]app.UseAuthorization(); [cite: 85]
[cite_start]app.MapControllers(); [cite: 86]

[cite_start]// MapHub DEVE vir após UseAuthorization [cite: 88, 90]
[cite_start]app.MapHub<PedidoHub>("/hubs/pedidos"); [cite: 88]

app.Run();

using MediatR;
using MassTransit;
using GestaoPedidos.Domain.Events;

[cite_start]public class AtualizarStatusPedidoCommandHandler : IRequestHandler<AtualizarStatusPedidoCommand, Unit> [cite: 104]
{
    [cite_start]private readonly IPedidoRepository _repo; [cite: 105]
    [cite_start]private readonly IPublishEndpoint _bus; [cite: 105]
    [cite_start]private readonly IPedidoCacheService _cache; [cite: 105]

    public AtualizarStatusPedidoCommandHandler(IPedidoRepository repo, IPublishEndpoint bus, IPedidoCacheService cache)
    {
        _repo = repo;
        _bus = bus;
        _cache = cache;
    }

    [cite_start]public async Task<Unit> Handle(AtualizarStatusPedidoCommand cmd, CancellationToken ct) [cite: 106, 109]
    {
        [cite_start]var pedido = await _repo.ObterPorIdAsync(cmd.PedidoId, ct) [cite: 110]
            [cite_start]?? throw new NotFoundException("Pedido não encontrado."); [cite: 111]

        [cite_start]var statusAnterior = pedido.Status.ToString(); [cite: 112, 113]
        [cite_start]pedido.AtualizarStatus(cmd.NovoStatus, cmd.Observacao); [cite: 113]
        [cite_start]await _repo.SalvarAsync(ct); [cite: 113]

        [cite_start]// Invalida cache do pedido [cite: 114]
        [cite_start]await _cache.InvalidateAsync(cmd.PedidoId, ct); [cite: 114]

        [cite_start]// Publica evento para o Consumer SignalR notificar em tempo real [cite: 115]
        [cite_start]await _bus.Publish(new PedidoStatusAlteradoEvent( [cite: 115]
            [cite_start]PedidoId: cmd.PedidoId, [cite: 116]
            [cite_start]StatusAnterior: statusAnterior, [cite: 117]
            [cite_start]NovoStatus: cmd.NovoStatus, [cite: 118]
            [cite_start]AlteradoEm: DateTime.UtcNow, [cite: 119]
            [cite_start]Observacao: cmd.Observacao [cite: 120]
        [cite_start]), ct); [cite: 121]

        [cite_start]return Unit.Value; [cite: 122]
    }
}

[cite_start]using Microsoft.AspNetCore.SignalR.Client; [cite: 178]
[cite_start]using Microsoft.AspNetCore.Mvc.Testing; [cite: 179]
using Microsoft.AspNetCore.SignalR;
using FluentAssertions;
using Xunit;

[cite_start]public class SignalRPedidoHubTests : IClassFixture<WebApplicationFactory<Program>> [cite: 180, 181]
{
    [cite_start]private readonly WebApplicationFactory<Program> _factory; [cite: 182]

    [cite_start]public SignalRPedidoHubTests(WebApplicationFactory<Program> factory) [cite: 183]
    {
        [cite_start]_factory = factory.WithWebHostBuilder(builder => [cite: 185]
        {
            [cite_start]builder.ConfigureServices(services => [cite: 186]
            {
                [cite_start]// Substituir Redis por InMemory para testes unitários do Hub [cite: 187]
                services.AddSignalR(); // sem Redis backplane nos testes [cite: 188]
            [cite_start]}); [cite: 189]
        [cite_start]}); [cite: 190]
    }

    [cite_start][Fact] [cite: 193]
    [cite_start]public async Task AssinarPedido_DeveReceberNotificacaoStatusAtualizado() [cite: 192]
    {
        [cite_start]// Arrange [cite: 195]
        [cite_start]var httpClient = _factory.CreateClient(); [cite: 198]
        
        [cite_start]var connection = new HubConnectionBuilder() [cite: 199]
            [cite_start].WithUrl("http://localhost/hubs/pedidos", opts => [cite: 200]
            {
                [cite_start]opts.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler(); [cite: 202, 204]
            [cite_start]}) [cite: 205]
            [cite_start].Build(); [cite: 206]

        [cite_start]var tcs = new TaskCompletionSource<PedidoStatusDto>(); [cite: 208]
        
        [cite_start]connection.On<PedidoStatusDto>("StatusAtualizado", dto => [cite: 209]
        {
            [cite_start]tcs.SetResult(dto); [cite: 212]
        [cite_start]}); [cite: 211]

        [cite_start]// Act [cite: 213]
        [cite_start]await connection.StartAsync(); [cite: 215]
        [cite_start]var pedidoId = Guid.NewGuid(); [cite: 216]
        [cite_start]await connection.InvokeAsync("AssinarPedido", pedidoId); [cite: 217]

        [cite_start]// Simular envio direto via IHubContext (em teste de integração) [cite: 218]
        [cite_start]var hubContext = _factory.Services.GetRequiredService<IHubContext<PedidoHub>>(); [cite: 219]
        
        [cite_start]await hubContext.Clients.Group(PedidoHub.GrupoKey(pedidoId)) [cite: 220]
            [cite_start].SendAsync("StatusAtualizado", new PedidoStatusDto( [cite: 220]
                [cite_start]pedidoId, "Em Preparo", "Enviado", [cite: 224] // Status anterior -> Novo Status
                [cite_start]DateTime.UtcNow, null)); [cite: 225]

        [cite_start]// Assert [cite: 226]
        [cite_start]var resultado = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5)); [cite: 228]
        
        [cite_start]resultado.Should().NotBeNull(); [cite: 229]
        [cite_start]resultado.Status.Should().Be("Enviado"); [cite: 230]
        [cite_start]resultado.PedidoId.Should().Be(pedidoId); [cite: 231]
        
        [cite_start]await connection.StopAsync(); [cite: 232]
    }
    
    // Dto Auxiliar para o Teste
    private record PedidoStatusDto(Guid PedidoId, string StatusAnterior, string Status, DateTime AlteradoEm, string? Observacao);
}

