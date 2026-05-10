namespace GestaoPedidos.API.Models
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoTotal { get; set; }
    }
}