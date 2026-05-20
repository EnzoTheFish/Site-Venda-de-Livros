namespace GestaoPedidos.API.Models;

public class CriarPedidoRequest
{
    public Guid ProductId { get; set; }
    public int Quantidade { get; set; }
}
