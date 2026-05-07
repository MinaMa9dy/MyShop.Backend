using Microsoft.AspNetCore.Http;
using MyShop.Application.DTOs.Profile;
using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IProfileService
    {
        Task<Result<ProfileDto>> GetUserByIdAsync(Guid userId);
        Task<Result<ProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        //Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<Result<ProfileDto>> UploadProfilePictureAsync(Guid userId, IFormFile photo);
        Task<Result<bool>> DeleteProfilePictureAsync(Guid userId);
        Task<Result<bool>> DeleteAccountAsync(Guid userId);
        //Task<Result<bool>> BanUserAsync(Guid userId);
        Task<Result<IEnumerable<ProfileDto>>> SearchUsersAsync(string query);
        Task<Result<IEnumerable<ProfileDto>>> GetAllUsersAsync();
        Task<Result<IEnumerable<ProfileDto>>> SearchCustomersAsync(string query);
        Task<Result<IEnumerable<ProfileDto>>> GetCustomersAsync();
    }
}
