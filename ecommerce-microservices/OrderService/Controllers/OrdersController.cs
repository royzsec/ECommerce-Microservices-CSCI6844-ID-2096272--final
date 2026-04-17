using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.DTOs;
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
            var orderDtos = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            });
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();
            var orderDto = new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto createOrderDto)
        {
            var customerExists = await _customerClient.CustomerExistsAsync(createOrderDto.CustomerId);
            if (!customerExists)
                return BadRequest("Customer does not exist");

            var product = await _productClient.GetProductAsync(createOrderDto.ProductId);
            if (product == null)
                return BadRequest("Product does not exist");

            if (product.Stock < createOrderDto.Quantity)
                return BadRequest("Insufficient stock");

            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                TotalPrice = product.Price * createOrderDto.Quantity,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _publisher.PublishOrderCreated(order.Id, order.ProductId, order.Quantity, order.TotalPrice);

            var response = new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, response);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            _publisher.PublishOrderCancelled(order.Id, order.ProductId, order.Quantity);

            return Ok(order);
        }
    }
}
