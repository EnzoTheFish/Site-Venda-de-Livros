using GestaoPedidos.Mensageria;
using MassTransit;

namespace GestaoPedidos.Notificacoes;

public class PedidoCriadoConsumer(ILogger<PedidoCriadoConsumer> logger) : IConsumer<PedidoCriado>
{
    public Task Consume(ConsumeContext<PedidoCriado> context)
    {
        var pedido = context.Message;

        logger.LogInformation(
            "Notificacao enviada para o pedido {PedidoId}. Produto: {ProductId}, quantidade: {Quantidade}, total: {PrecoTotal}",
            pedido.PedidoId,
            pedido.ProductId,
            pedido.Quantidade,
            pedido.PrecoTotal);

        return Task.CompletedTask;
    }
}
