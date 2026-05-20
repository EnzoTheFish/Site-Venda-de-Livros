namespace GestaoPedidos.Mensageria;

public record PedidoCriado(
    Guid PedidoId,
    Guid ProductId,
    int Quantidade,
    decimal PrecoTotal,
    DateTimeOffset CriadoEm);
