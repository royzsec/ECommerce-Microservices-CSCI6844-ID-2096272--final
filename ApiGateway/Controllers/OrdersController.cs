using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        
        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _httpClient.GetAsync("http://orderservice:8080/api/orders");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"http://orderservice:8080/api/orders/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement request)
        {
            var json = request.GetRawText();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://orderservice:8080/api/orders", content);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _httpClient.PutAsync($"http://orderservice:8080/api/orders/{id}/cancel", null);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"http://orderservice:8080/api/orders/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
