using Microsoft.AspNetCore.Http;
using MyShop.CORE.Dtos.Profile;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
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
        Task<PageResult<ProfileDto>> SearchUsersAsync(string query);
        Task<PageResult<ProfileDto>> GetAllUsersAsync();
        Task<PageResult<ProfileDto>> SearchCustomersAsync(string query);
        Task<PageResult<ProfileDto>> GetCustomersAsync();
    }
}
