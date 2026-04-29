using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MyShop.CORE.Dtos.Profile;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Core.Interfaces;
using MyShop.CORE.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyShop.CORE.Helpers;

namespace MyShop.CORE.Implmentations
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
        //        return Result<UserProfileDto>.Failure("User Not Found", "404");
        //    }
        //    user.IsBanned = true;
        //    await _unitOfWork.CompleteAsync();
        //    return Result<UserProfileDto>.Success(_mapper.Map<UserProfileDto>(user));
        //}

        

        public async Task<Result<bool>> DeleteAccountAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId);
            if (user == null)
            {
                return Result<bool>.Failure("User Not Found", "404");
            }
            
            _unitOfWork.Users.Delete(user);
            var result = await _unitOfWork.CompleteAsync();
            if(result == 0)
            {
                return Result<bool>.Failure("Failed to delete account", "400");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteProfilePictureAsync(Guid userId)
        {
            var oldPhoto = await _unitOfWork.UserPhotos.FindAsync(p => p.UserId == userId);
            if (oldPhoto is not null)
            {
                _fileService.DeleteFile(oldPhoto.FileName, UserPhotosFolder);
                _unitOfWork.UserPhotos.Delete(oldPhoto);
                await _unitOfWork.CompleteAsync();
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure("Profile picture not found", "404");
        }

        public async Task<Result<ProfileDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId, includes: new[] { nameof(ApplicationUser.userPhoto) });

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", "404");
            }

            var userProfile = _mapper.Map<ProfileDto>(user);
            return Result<ProfileDto>.Success(userProfile);
        }
        

        public async Task<Result<ProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId, includes: new[] { nameof(ApplicationUser.userPhoto) });

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", "404");
            }
            _mapper.Map(dto, user);
            _unitOfWork.Users.Update(user);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return Result<ProfileDto>.Failure("Failed to update profile", "400");
            }
            return Result<ProfileDto>.Success(_mapper.Map<ProfileDto>(user));
        }

        public async Task<Result<ProfileDto>> UploadProfilePictureAsync(Guid userId, IFormFile photo)
        {
            if (photo.Length > 3 * (1 << 20))
            {
                return Result<ProfileDto>.Failure("The photo size is bigger than 3 Mb", "400");
            }

            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId, includes: new[] { nameof(ApplicationUser.userPhoto) });

            if (user == null)
            {
                return Result<ProfileDto>.Failure("User Not Found", "404");
            }

            var saveResult = await _fileService.SaveFileAsync(photo, UserPhotosFolder);
            if (!saveResult.IsSuccess)
            {
                return Result<ProfileDto>.Failure(saveResult.Error?.Message ?? "Failed to save file", "400");
            }

            var trustedFileName = saveResult.Data;

            // Delete old photo if exists
            var oldPhoto = await _unitOfWork.UserPhotos.FindAsync(p => p.UserId == userId);
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

        public async Task<PageResult<ProfileDto>> SearchUsersAsync(string query)
        {
            var users = await _unitOfWork.Users.FindAllAsync(
                u => u.Email.Contains(query) || (u.FirstName + " " + u.LastName).Contains(query),
                includes: new[] { nameof(ApplicationUser.userPhoto) }
            );

            return new PageResult<ProfileDto>
            {
                Items = _mapper.Map<List<ProfileDto>>(users.ToList()),
                TotalItems = users.Count()
            };
        }

        public async Task<PageResult<ProfileDto>> SearchCustomersAsync(string query)
        {
            var customerIds = await _identityService.GetUserIdsInRoleAsync("Customer");
            var users = await _unitOfWork.Users.FindAllAsync(
                u => customerIds.Contains(u.Id.ToString()) && (u.Email.Contains(query) || (u.FirstName + " " + u.LastName).Contains(query)),
                includes: new[] { nameof(ApplicationUser.userPhoto) }
            );

            return new PageResult<ProfileDto>
            {
                Items = _mapper.Map<List<ProfileDto>>(users.ToList()),
                TotalItems = users.Count()
            };
        }

        public async Task<PageResult<ProfileDto>> GetCustomersAsync()
        {
            var countTask = _unitOfWork.Customers.CountAsync();

            var customersTask = _unitOfWork.Customers.FindAllAsync(
                take: 5,
                includes: new[] { nameof(Customer.User) }
            );

            await Task.WhenAll(countTask, customersTask);

            var totalCount = await countTask;
            var customers = (List<Customer>) await customersTask;

            return new PageResult<ProfileDto>
            {
                Items = _mapper.Map<List<ProfileDto>>(
                    customers.Select(c => c.User)
                ),
                TotalItems = totalCount,
                Page = 1,
                PageSize = 8
            };
        }

        public async Task<PageResult<ProfileDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAllAsync(includes: new[] { nameof(ApplicationUser.userPhoto) });

            return new PageResult<ProfileDto>
            {
                Items = _mapper.Map<List<ProfileDto>>(users.ToList()),
                TotalItems = users.Count(),
                Page = 1,
                PageSize = 8
            };
        }
    }
}
