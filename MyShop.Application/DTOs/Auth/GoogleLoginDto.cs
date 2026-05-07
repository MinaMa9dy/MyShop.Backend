using System.ComponentModel.DataAnnotations;

namespace MyShop.Application.DTOs
{
    public class GoogleLoginDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
