using System.Net.Http.Json;
using BlazorFrontend.Models;
using System.Text.Json;


namespace BlazorFrontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("/api/products");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductDto>>(content, _options) ?? new List<ProductDto>();
        }

        public async Task<ProductDto?> CreateProductAsync(ProductDto product)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/products", product);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(content, _options);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, ProductDto product)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/products/{id}", product);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(content, _options);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/products/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync("/api/customers");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CustomerDto>>(content, _options) ?? new List<CustomerDto>();
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CustomerDto customer)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/customers", customer);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerDto>(content, _options);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerDto customer)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/customers/{id}", customer);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerDto>(content, _options);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/customers/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync("/api/orders");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderResponseDto>>(content, _options) ?? new List<OrderResponseDto>();
        }

        public async Task<OrderResponseDto?> CreateOrderAsync(CreateOrderDto order)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/orders", order);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderResponseDto>(content, _options);
        }

        public async Task<OrderResponseDto?> CancelOrderAsync(int id)
        {
            var response = await _httpClient.PutAsync($"/api/orders/{id}/cancel", null);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderResponseDto>(content, _options);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/orders/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
