namespace MyShop.Application.DTOs.Identity
{
    public class CreateIdentityUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
