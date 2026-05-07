using System.ComponentModel.DataAnnotations;

namespace MyShop.Application.DTOs
{
    public class ResendEmailConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
