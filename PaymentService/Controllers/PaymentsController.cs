using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.DTOs;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentDbContext _context;

        public PaymentsController(PaymentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments.ToListAsync();
            var response = payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Status = p.Status,
                PaymentDate = p.PaymentDate
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });
            
            var response = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return Ok(response);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment == null)
                return NotFound(new { message = "Payment not found for this order" });
            
            var response = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentRequestDto request)
        {
            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Status = "Pending",
                PaymentDate = DateTime.UtcNow
            };
            
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            
            var response = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, response);
        }

        [HttpPut("{id}/process")]
        public async Task<IActionResult> Process(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            payment.Status = "Completed";
            await _context.SaveChangesAsync();
            
            var response = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });
            
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Payment deleted successfully" });
        }
    }
}
