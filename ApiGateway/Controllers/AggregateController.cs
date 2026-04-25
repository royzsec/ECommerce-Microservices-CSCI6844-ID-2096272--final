using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/aggregate")]
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
            try
            {
                // Step 1: Get Order
                var orderResponse = await _httpClient.GetAsync($"http://orderservice:8080/api/orders/{orderId}");
                if (!orderResponse.IsSuccessStatusCode)
                {
                    return NotFound(new { error = $"Order {orderId} not found" });
                }

                var orderJson = await orderResponse.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<OrderData>(orderJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Step 2: Get Customer
                var customerResponse = await _httpClient.GetAsync($"http://customerservice:8080/api/customers/{order.CustomerId}");
                var customerJson = await customerResponse.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<CustomerData>(customerJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Step 3: Get Product
                var productResponse = await _httpClient.GetAsync($"http://productservice:8080/api/products/{order.ProductId}");
                var productJson = await productResponse.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductData>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Step 4: Get Payment
                var paymentResponse = await _httpClient.GetAsync($"http://paymentservice:8080/api/payments/order/{orderId}");
                PaymentData payment = null;
                if (paymentResponse.IsSuccessStatusCode)
                {
                    var paymentJson = await paymentResponse.Content.ReadAsStringAsync();
                    payment = JsonSerializer.Deserialize<PaymentData>(paymentJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // Step 5: Return combined result
                var result = new
                {
                    Order = new
                    {
                        order.Id,
                        order.CustomerId,
                        order.ProductId,
                        order.Quantity,
                        order.TotalPrice,
                        order.Status,
                        order.CreatedAt
                    },
                    Customer = customer != null ? new
                    {
                        customer.Id,
                        customer.Name,
                        customer.Email
                    } : null,
                    Product = product != null ? new
                    {
                        product.Id,
                        product.Name,
                        product.Price,
                        product.Stock
                    } : null,
                    Payment = payment != null ? new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.Amount,
                        payment.Status,
                        payment.PaymentDate
                    } : null
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private class OrderData
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private class CustomerData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        private class ProductData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        private class PaymentData
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public DateTime PaymentDate { get; set; }
        }
    }
}
