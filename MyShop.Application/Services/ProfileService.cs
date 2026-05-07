using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyShop.Application.DTOs.Profile;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Application.Interfaces.Auth;
using MyShop.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyShop.Application.Common;

namespace MyShop.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IIdentityService _identityService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private const string UserPhotosFolder = "UserPhotos";
        public ProfileService(IIdentityService identityService, IMapper mapper, IUnitOfWork unitOfWork, IAuthService authService, IFileService fileService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _fileService = fileService;
            _identityService = identityService;
        }

        //public async Task<Result<UserProfileDto>> BanUser(Guid userId)
        //{
        //    var user = await _profileService.FindByIdAsync(userId.ToString());
        //    if (user == null)
        //    {
        //        return Result<UserProfileDto>.Failure("User Not Found", ErrorCode.NOT_FOUND);
        //    }
        //    user.IsBanned = true;
        //    await _unitOfWork.CompleteAsync();
        //    return Result<UserProfileDto>.Ok(_mapper.Map<UserProfileDto>(user));
        //}

        

        public async Task<Result<bool>> DeleteAccountAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.Query(false).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Result<bool>.Failure("User Not Found", ErrorCode.NOT_FOUND);
            }
            
            _unitOfWork.Users.Delete(user);
            var result = await _unitOfWork.CompleteAsync();
            if(result == 0)
            {
                return Result<bool>.Failure("Failed to delete account", ErrorCode.SERVER_ERROR);
            }
            return Result<bool>.Ok(true, "Account deleted successfully");
        }

        public async Task<Result<bool>> DeleteProfilePictureAsync(Guid userId)
        {
            var oldPhoto = await _unitOfWork.UserPhotos.Query(false).FirstOrDefaultAsync(p => p.UserId == userId);
            if (oldPhoto is not null)
            {
                _fileService.DeleteFile(oldPhoto.FileName, UserPhotosFolder);
                _unitOfWork.UserPhotos.Delete(oldPhoto);
                await _unitOfWork.CompleteAsync();
                return Result<bool>.Ok(true, "Profile picture deleted successfully");
            }
            return Result<bool>.Failure("Profile picture not found", ErrorCode.NOT_FOUND);
        }

        public async Task<Result<ProfileDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.Query()
                .Include(u => u.userPhoto)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", ErrorCode.NOT_FOUND);
            }

            var userProfile = _mapper.Map<ProfileDto>(user);
            return Result<ProfileDto>.Ok(userProfile);
        }
        

        public async Task<Result<ProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _unitOfWork.Users.Query(false)
                .Include(u => u.userPhoto)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", ErrorCode.NOT_FOUND);
            }
            _mapper.Map(dto, user);
            _unitOfWork.Users.Update(user);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return Result<ProfileDto>.Failure("Failed to update profile", ErrorCode.SERVER_ERROR);
            }
            return Result<ProfileDto>.Ok(_mapper.Map<ProfileDto>(user));
        }

        public async Task<Result<ProfileDto>> UploadProfilePictureAsync(Guid userId, IFormFile photo)
        {
            if (photo.Length > 3 * (1 << 20))
            {
                return Result<ProfileDto>.Failure("The photo size is bigger than 3 Mb", ErrorCode.VALIDATION_ERROR);
            }
            
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", ErrorCode.NOT_FOUND);
            }

            var saveResult = await _fileService.SaveFileAsync(photo, UserPhotosFolder);
            if (!saveResult.Success)
            {
                return Result<ProfileDto>.Failure(saveResult.Error?.Message ?? "Failed to save file", ErrorCode.VALIDATION_ERROR);
            }

            var trustedFileName = saveResult.Data;

            // Delete old photo if exists
            var oldPhoto = await _unitOfWork.UserPhotos.Query(false).FirstOrDefaultAsync(p => p.UserId == userId);
            if (oldPhoto is not null)
            {
                _fileService.DeleteFile(oldPhoto.FileName, UserPhotosFolder);
                _unitOfWork.UserPhotos.Delete(oldPhoto);
            }

            var userPhoto = new UserPhoto
            {
                UserId = userId,
                FileName = trustedFileName,
                RelativePath = Path.Combine(UserPhotosFolder, trustedFileName),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserPhotos.AddAsync(userPhoto);
            await _unitOfWork.CompleteAsync();

            return await GetUserByIdAsync(userId);
        }

        public async Task<Result<IEnumerable<ProfileDto>>> SearchUsersAsync(string query)
        {
            var users = await _unitOfWork.Users.Query()
                .Where(u => u.Email.Contains(query) || (u.FirstName + " " + u.LastName).Contains(query))
                .Include(u => u.userPhoto)
                .ToListAsync();

            var meta = new Meta { Total = users.Count(), Page = 1, PerPage = users.Count() > 0 ? users.Count() : 1 };
            return Result<IEnumerable<ProfileDto>>.Ok(_mapper.Map<List<ProfileDto>>(users.ToList()), "Success", meta);
        }

        public async Task<Result<IEnumerable<ProfileDto>>> SearchCustomersAsync(string query)
        {
            var customers = await _unitOfWork.Customers.Query()
                .Where(u => u.User.Email.Contains(query) || (u.User.FirstName + " " + u.User.LastName).Contains(query))
                .Include(u => u.User)
                    .ThenInclude(u => u.userPhoto)
                .ToListAsync();

            var meta = new Meta { Total = customers.Count(), Page = 1, PerPage = customers.Count() > 0 ? customers.Count() : 1 };
            return Result<IEnumerable<ProfileDto>>.Ok(customers.Select(c => _mapper.Map<ProfileDto>(c.User)).ToList(), "Success", meta);
        }

        public async Task<Result<IEnumerable<ProfileDto>>> GetCustomersAsync()
        {
            var count = await _unitOfWork.Customers.Query().CountAsync();

            var customers = await _unitOfWork.Customers.Query()
                .Take(8)
                .Include(u => u.User)
                    .ThenInclude(u => u.userPhoto)
                .ToListAsync();

            var meta = new Meta { Total = count, Page = 1, PerPage = 8 };
            return Result<IEnumerable<ProfileDto>>.Ok(_mapper.Map<List<ProfileDto>>(customers.Select(c => c.User)), "Success", meta);
        }

        public async Task<Result<IEnumerable<ProfileDto>>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.Query()
                .Include(u => u.userPhoto)
                .ToListAsync();

            var meta = new Meta { Total = users.Count(), Page = 1, PerPage = 8 };
            return Result<IEnumerable<ProfileDto>>.Ok(_mapper.Map<List<ProfileDto>>(users.ToList()), "Success", meta);
        }
    }
}
