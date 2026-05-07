using MyShop.Domain.Settings;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;
using MyShop.Domain.Identity;
using MyShop.Application.DTOs;
using MyShop.Application.Common.ResultPattern;
using Google.Apis.Auth;
using MyShop.Domain.Enums;
using MyShop.Application.Interfaces;
using MyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly GoogleSettings _googleSettings;
        private readonly ClientSettings _clientSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IIdentityService identityService,
            ITokenService tokenService,
            IEmailService emailService,
            IOptions<JwtSettings> jwtSettings,
            IOptions<GoogleSettings> googleSettings,
            IOptions<ClientSettings> googleClientSettings, // Changed name to avoid conflict if needed, or keep it
            IOptions<ClientSettings> clientSettings,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _googleSettings = googleSettings.Value;
            _clientSettings = clientSettings.Value;
            _unitOfWork = unitOfWork;
        }

        // ─── Register ────────────────────────────────────────────────────────────

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser is not null)
                return Result<AuthResponseDto>.Failure("Email is already taken.", ErrorCode.DUPLICATE_ENTRY);

            var createResult = await _identityService.CreateUserAsync(new MyShop.Application.DTOs.Identity.CreateIdentityUserDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                EmailConfirmed = false
            });

            if (!createResult.Success)
                return Result<AuthResponseDto>.Failure(createResult.Error!.Message, ErrorCode.VALIDATION_ERROR);

            var userId = createResult.Data;
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            
            await _identityService.AddToRoleAsync(userId, RoleOptions.Customer.ToString());
            
            Customer customer = new Customer();
            customer.UserId = user.Id;

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();


            await SendConfirmationEmailAsync(user);
            
            return Result<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Email = user.Email!,
                RequiresEmailConfirmation = true,
                Message = "Registration successful. Please check your email to confirm your account before logging in."
            }, "Registration successful");
        }

        // ─── Login ───────────────────────────────────────────────────────────────

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return Result<AuthResponseDto>.Failure("Invalid credentials.", ErrorCode.UNAUTHENTICATED);

            if (!await _identityService.IsEmailConfirmedAsync(user.Id.ToString()))
                return Result<AuthResponseDto>.Failure("Email is not confirmed. Please check your inbox.", ErrorCode.UNAUTHENTICATED);
            
            if(user.IsBanned)
                return Result<AuthResponseDto>.Failure("User account is banned.", ErrorCode.FORBIDDEN);

            var result = await _identityService.CheckPasswordAsync(user.Id.ToString(), dto.Password);
            if (!result)
                return Result<AuthResponseDto>.Failure("Invalid credentials.", ErrorCode.UNAUTHENTICATED);

            var roles = await _identityService.GetRolesAsync(user.Id.ToString());
            var (accessToken, refreshToken) = await GenerateAndPersistTokensAsync(user, roles);

            return Result<AuthResponseDto>.Ok(BuildAuthResponse(user, roles, accessToken, refreshToken), "Login successful");
        }

        // ─── Refresh Token ───────────────────────────────────────────────────────

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal is null)
                return Result<AuthResponseDto>.Failure("Invalid access token.", ErrorCode.UNAUTHENTICATED);

            var userId = principal.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier
                                  || c.Type == "sub")?.Value;

            if (userId == null)
                return Result<AuthResponseDto>.Failure("Invalid token claims.", ErrorCode.UNAUTHENTICATED);

            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            if (user == null)
                return Result<AuthResponseDto>.Failure("User not found.", ErrorCode.NOT_FOUND);
            if (user.IsBanned)
               return Result<AuthResponseDto>.Failure("User account is banned.", ErrorCode.FORBIDDEN);
            var refreshtoken = await _unitOfWork.RefreshTokens.Query().FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);
            if (user is null ||
                refreshtoken is null ||
                refreshtoken.ExpiryDate < DateTime.UtcNow)
            {
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.", ErrorCode.UNAUTHENTICATED);
            }

            var roles = await _identityService.GetRolesAsync(user.Id.ToString());
            var (accessToken, refreshToken) = await GenerateAndPersistTokensAsync(user, roles);

            return Result<AuthResponseDto>.Ok(BuildAuthResponse(user, roles, accessToken, refreshToken), "Token refreshed");
        }

        // ─── Confirm Email ───────────────────────────────────────────────────────

        public async Task<Result<string>> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == Guid.Parse(dto.UserId));
            if (user is null)
                return Result<string>.Failure("User not found.", ErrorCode.NOT_FOUND);

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _identityService.ConfirmEmailAsync(user.Id.ToString(), decodedToken);
            if (!result.Success)
                return Result<string>.Failure(result.Error!.Message, ErrorCode.VALIDATION_ERROR);

            return Result<string>.Ok("Email confirmed successfully.");
        }

        // ─── Resend Confirmation ─────────────────────────────────────────────────

        public async Task<Result<string>> ResendEmailConfirmationAsync(ResendEmailConfirmationDto dto)
        {
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return Result<string>.Ok("If the email exists, a confirmation link has been sent.");

            if (await _identityService.IsEmailConfirmedAsync(user.Id.ToString()))
                return Result<string>.Failure("Email is already confirmed.", ErrorCode.VALIDATION_ERROR);

            await SendConfirmationEmailAsync(user);

            return Result<string>.Ok("Confirmation email sent. Please check your inbox.");
        }

        // ─── Forgot Password ─────────────────────────────────────────────────────

        public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            if (user is null || !await _identityService.IsEmailConfirmedAsync(user.Id.ToString()))
                return Result<string>.Ok("If the email exists, a reset link has been sent.");
            if(user.IsBanned)
                return Result<string>.Failure("User account is banned.", ErrorCode.FORBIDDEN);
            var token = await _identityService.GeneratePasswordResetTokenAsync(user.Id.ToString());
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = $"{_clientSettings.BaseUrl}/ar/auth/reset-password?userId={user.Id}&token={encodedToken}";
            var body = $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>";

            await _emailService.SendEmailAsync(user.Email!, "Reset Your Password", body);

            return Result<string>.Ok("If the email exists, a reset link has been sent.");
        }

        // ─── Reset Password ──────────────────────────────────────────────────────

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _unitOfWork.Users.Query(false).FirstOrDefaultAsync(u => u.Id == Guid.Parse(dto.UserId));
            if (user is null)
                return Result<string>.Failure("Invalid request.", ErrorCode.VALIDATION_ERROR);
            if(user.IsBanned)
                return Result<string>.Failure("User account is banned.", ErrorCode.FORBIDDEN);
            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _identityService.ResetPasswordAsync(user.Id.ToString(), decodedToken, dto.NewPassword);
            if (!result.Success)
                return Result<string>.Failure(result.Error!.Message, ErrorCode.VALIDATION_ERROR);

            return Result<string>.Ok("Password reset successfully.");
        }

        // ─── Google Login ────────────────────────────────────────────────────────

        public async Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _googleSettings.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Token, settings);
                
                var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Email == payload.Email);
                
                if (user == null)
                {
                    var createResult = await _identityService.CreateUserAsync(new MyShop.Application.DTOs.Identity.CreateIdentityUserDto
                    {
                        Email = payload.Email,
                        FirstName = payload.GivenName ?? "Unknown",
                        LastName = payload.FamilyName ?? "Unknown",
                        EmailConfirmed = true
                    });

                    if (!createResult.Success)
                    {
                        return Result<AuthResponseDto>.Failure(createResult.Error!.Message, ErrorCode.VALIDATION_ERROR);
                    }
                    
                    user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == Guid.Parse(createResult.Data));

                    await _identityService.AddToRoleAsync(user.Id.ToString(), RoleOptions.Customer.ToString());
                    Customer customer = new Customer();
                    customer.UserId = user.Id;
                    await _unitOfWork.Customers.AddAsync(customer);
                    await _unitOfWork.CompleteAsync();
                }
                if(user.IsBanned)
                    return Result<AuthResponseDto>.Failure("User account is banned.", ErrorCode.FORBIDDEN);

                var roles = await _identityService.GetRolesAsync(user.Id.ToString());
                var (accessToken, refreshToken) = await GenerateAndPersistTokensAsync(user, roles);

                return Result<AuthResponseDto>.Ok(BuildAuthResponse(user, roles, accessToken, refreshToken), "Google login successful");
            }
            catch (InvalidJwtException)
            {
                return Result<AuthResponseDto>.Failure("Invalid Google token.", ErrorCode.UNAUTHENTICATED);
            }
            catch (Exception)
            {
                return Result<AuthResponseDto>.Failure("Something went wrong while validating the token.", ErrorCode.SERVER_ERROR);
            }
        }

        // ─── Private Helpers ─────────────────────────────────────────────────────

        private async Task<(string accessToken, string refreshToken)> GenerateAndPersistTokensAsync(
            ApplicationUser user, IList<string> roles)
        {
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            

            await _unitOfWork.CompleteAsync();

            return (accessToken, refreshToken);
        }

        private AuthResponseDto BuildAuthResponse(
            ApplicationUser user, IList<string> roles,
            string accessToken, string refreshToken)
        {
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Email = user.Email!,
                Roles = roles,
            };
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var token = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id.ToString());
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmLink = $"{_clientSettings.BaseUrl}/ar/auth/confirm-email?userId={user.Id}&token={encodedToken}";
            var body = $"<p>Please confirm your email by clicking the link below:</p><p><a href='{confirmLink}'>Confirm Email</a></p>";

            await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);
        }
    }
}
