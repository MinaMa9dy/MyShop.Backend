using System.ComponentModel.DataAnnotations;

namespace MyShop.Core.Dtos
{
    public class ResendEmailConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
