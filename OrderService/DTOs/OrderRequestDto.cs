using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs
{
    public class OrderRequestDto
    {
        [Required]
        [Range(1, 999999)]
        public int CustomerId { get; set; }
        
        [Required]
        [Range(1, 999999)]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, 999999)]
        public int Quantity { get; set; }
    }
}
