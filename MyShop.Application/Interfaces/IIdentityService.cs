using MyShop.Application.DTOs.Identity;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IIdentityService
    {
        // User Creation
        Task<Result<string>> CreateUserAsync(CreateIdentityUserDto dto);

        // Password
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<Result<bool>> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string userId);

        // Email Confirmation
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<Result<bool>> ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);

        // Roles
        Task<Result<bool>> AddToRoleAsync(string userId, string role);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<List<string>> GetRolesAsync(string userId);
        Task<List<string>> GetUserIdsInRoleAsync(string role);
    }
}
