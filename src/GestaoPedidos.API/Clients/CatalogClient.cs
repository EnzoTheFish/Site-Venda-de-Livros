using System.Net.Http.Json;
using GestaoPedidos.API.Models;

namespace GestaoPedidos.API.Clients
{
    public class CatalogClient : ICatalogClient
    {
        private readonly HttpClient _http;

        public CatalogClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ProductDto?> GetProductById(Guid id)
        {
            var response = await _http.GetAsync($"/api/v1/products/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
    }
}