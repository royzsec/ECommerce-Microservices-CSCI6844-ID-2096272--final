using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        
        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _httpClient.GetAsync("http://productservice:8080/api/products");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"http://productservice:8080/api/products/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement request)
        {
            var json = request.GetRawText();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://productservice:8080/api/products", content);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] JsonElement request)
        {
            var json = request.GetRawText();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"http://productservice:8080/api/products/{id}", content);
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"http://productservice:8080/api/products/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
