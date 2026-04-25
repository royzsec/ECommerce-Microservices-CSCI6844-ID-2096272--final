using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ICustomerClient _customerClient;
        private readonly IProductClient _productClient;
        private readonly RabbitMQPublisher _publisher;

        public OrdersController(OrderDbContext context, ICustomerClient customerClient, IProductClient productClient, RabbitMQPublisher publisher)
        {
            _context = context;
            _customerClient = customerClient;
            _productClient = productClient;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var customerExists = await _customerClient.CustomerExistsAsync(request.CustomerId);
            if (!customerExists)
                return BadRequest(new { message = "Customer does not exist" });

            var product = await _productClient.GetProductAsync(request.ProductId);
            if (product == null)
                return BadRequest(new { message = "Product does not exist" });

            if (product.Stock < request.Quantity)
                return BadRequest(new { message = "Insufficient stock" });

            var order = new Order
            {
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalPrice = product.Price * request.Quantity,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _publisher.PublishOrderCreated(order.Id, order.ProductId, order.Quantity, order.TotalPrice);

            return Ok(order);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.Status == "Cancelled")
                return BadRequest(new { message = "Order already cancelled" });

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            _publisher.PublishOrderCancelled(order.Id, order.ProductId, order.Quantity);

            return Ok(new { message = "Order cancelled successfully", order });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order deleted successfully" });
        }

        public class CreateOrderRequest
        {
            public int CustomerId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
