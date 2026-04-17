using System.Net;
using System.Text.Json;

namespace OrderService.Services
{
    public class ProductClient : IProductClient
    {
        private readonly HttpClient _httpClient;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<ProductInfo> GetProductAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductInfo>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
    }
}
