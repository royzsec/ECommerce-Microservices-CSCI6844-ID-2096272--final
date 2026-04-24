using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        
        public PaymentsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://paymentservice:8080/api/payments");
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch
            {
                return Ok("[]");
            }
        }
        
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://paymentservice:8080/api/payments/order/{orderId}");
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch
            {
                return Ok("null");
            }
        }
    }
}
