using System.ComponentModel.DataAnnotations;

namespace CustomerService.DTOs
{
    public class CustomerRequestDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
