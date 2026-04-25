using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.DTOs;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerDbContext _context;

        public CustomersController(CustomerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();
            var response = customers.Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            
            var response = new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequestDto request)
        {
            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email
            };
            
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            
            var response = new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            };
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerRequestDto request)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            
            customer.Name = request.Name;
            customer.Email = request.Email;
            await _context.SaveChangesAsync();
            
            var response = new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Customer deleted successfully" });
        }
    }
}
