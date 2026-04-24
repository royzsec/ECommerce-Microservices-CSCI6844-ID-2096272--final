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
            try
            {
                var response = await _httpClient.GetAsync("/api/products");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Products response: {content}");
                
                if (string.IsNullOrEmpty(content) || content == "[]")
                    return new List<ProductDto>();
                
                return JsonSerializer.Deserialize<List<ProductDto>>(content, _options) ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products: {ex.Message}");
                return new List<ProductDto>();
            }
        }

        public async Task<ProductDto?> CreateProductAsync(ProductDto product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/products", product);
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(content, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/customers");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Customers response: {content}");
                
                if (string.IsNullOrEmpty(content) || content == "[]")
                    return new List<CustomerDto>();
                
                return JsonSerializer.Deserialize<List<CustomerDto>>(content, _options) ?? new List<CustomerDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customers: {ex.Message}");
                return new List<CustomerDto>();
            }
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CustomerDto customer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/customers", customer);
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CustomerDto>(content, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/orders");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Orders response: {content}");
                
                if (string.IsNullOrEmpty(content) || content == "[]")
                    return new List<OrderResponseDto>();
                
                return JsonSerializer.Deserialize<List<OrderResponseDto>>(content, _options) ?? new List<OrderResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting orders: {ex.Message}");
                return new List<OrderResponseDto>();
            }
        }

        public async Task<OrderResponseDto?> CreateOrderAsync(CreateOrderDto order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/orders", order);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create order response: {content}");
                return JsonSerializer.Deserialize<OrderResponseDto>(content, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                return null;
            }
        }
    }
}
