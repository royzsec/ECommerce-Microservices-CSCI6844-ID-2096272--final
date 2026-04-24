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
                var orderResponse = await _httpClient.GetAsync($"http://orderservice:8080/api/orders/{orderId}");
                if (!orderResponse.IsSuccessStatusCode)
                    return NotFound("Order not found");
                    
                var orderJson = await orderResponse.Content.ReadAsStringAsync();
                
                return Ok(new { Order = orderJson });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
