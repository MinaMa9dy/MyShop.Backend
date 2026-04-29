using System.ComponentModel.DataAnnotations;

namespace MyShop.Core.Dtos
{
    public class GoogleLoginDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
