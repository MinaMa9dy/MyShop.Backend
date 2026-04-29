
using MyShop.Core.Dtos;
using MyShop.CORE.Helpers.ResultPattern;

namespace Identity.Core.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<Result<string>> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<Result<string>> ResendEmailConfirmationAsync(ResendEmailConfirmationDto dto);
        Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginDto dto);
        
    }
}
