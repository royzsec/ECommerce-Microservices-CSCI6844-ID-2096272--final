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
            var customerDtos = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            });
            return Ok(customerDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();
            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            };
            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
    }
}
