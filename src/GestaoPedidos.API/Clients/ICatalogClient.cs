namespace GestaoPedidos.API.Clients;
using GestaoPedidos.API.Models;

    public interface ICatalogClient
    {
        Task<ProductDto?> GetProductById(Guid id);
    }
