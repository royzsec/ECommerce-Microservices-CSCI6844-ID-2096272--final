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
            try
            {
                var response = await _httpClient.GetAsync("http://productservice:8080/api/products");
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch
            {
                return Ok("[]");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement request)
        {
            try
            {
                var json = request.GetRawText();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://productservice:8080/api/products", content);
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
