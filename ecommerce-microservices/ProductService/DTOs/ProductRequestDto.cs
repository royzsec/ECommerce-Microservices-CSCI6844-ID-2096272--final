using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class ProductRequestDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, 999999)]
        public int Stock { get; set; }
    }
}
