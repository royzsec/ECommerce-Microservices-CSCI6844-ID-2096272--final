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
            var response = await _httpClient.GetAsync("http://paymentservice:8080/api/payments");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"http://paymentservice:8080/api/payments/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var response = await _httpClient.GetAsync($"http://paymentservice:8080/api/payments/order/{orderId}");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement request)
        {
            var json = request.GetRawText();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://paymentservice:8080/api/payments", content);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpPut("{id}/process")]
        public async Task<IActionResult> Process(int id)
        {
            var response = await _httpClient.PutAsync($"http://paymentservice:8080/api/payments/{id}/process", null);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"http://paymentservice:8080/api/payments/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
