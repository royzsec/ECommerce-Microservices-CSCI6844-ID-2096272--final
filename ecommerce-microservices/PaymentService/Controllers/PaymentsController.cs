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
            var paymentDtos = payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Status = p.Status,
                PaymentDate = p.PaymentDate
            });
            return Ok(paymentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();
            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return Ok(paymentDto);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment == null)
                return NotFound();
            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
            return Ok(paymentDto);
        }
    }
}
