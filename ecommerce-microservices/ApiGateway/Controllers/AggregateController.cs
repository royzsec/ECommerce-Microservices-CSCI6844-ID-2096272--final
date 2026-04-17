using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("gateway/aggregate")]
    public class AggregateController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AggregateController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("order-details/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var orderResponse = await _httpClient.GetAsync($"http://orderservice:8080/api/orders/{orderId}");
            if (!orderResponse.IsSuccessStatusCode)
                return NotFound("Order not found");

            var orderJson = await orderResponse.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<OrderDto>(orderJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var customerResponse = await _httpClient.GetAsync($"http://customerservice:8080/api/customers/{order.CustomerId}");
            var customerJson = await customerResponse.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<CustomerDto>(customerJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var productResponse = await _httpClient.GetAsync($"http://productservice:8080/api/products/{order.ProductId}");
            var productJson = await productResponse.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var paymentResponse = await _httpClient.GetAsync($"http://paymentservice:8080/api/payments/order/{orderId}");
            PaymentDto payment = null;
            if (paymentResponse.IsSuccessStatusCode)
            {
                var paymentJson = await paymentResponse.Content.ReadAsStringAsync();
                payment = JsonSerializer.Deserialize<PaymentDto>(paymentJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var result = new
            {
                Order = order,
                Customer = customer,
                Product = product,
                Payment = payment
            };

            return Ok(result);
        }

        public class OrderDto
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
        }

        public class CustomerDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        public class PaymentDto
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
        }
    }
}
