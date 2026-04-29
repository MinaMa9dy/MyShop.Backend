namespace MyShop.Core.Dtos
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = [];
        public bool RequiresEmailConfirmation { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
