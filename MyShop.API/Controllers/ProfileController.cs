using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Profile;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Interfaces;
using System.Security.Claims;

namespace MyShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfileById(Guid userId)
        {
            var result = await _profileService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.UpdateProfileAsync(userId, dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPost("Upload-Image")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.UploadProfilePictureAsync(userId, file);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("Delete-Image")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.DeleteProfilePictureAsync(userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("Delete-Account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.DeleteAccountAsync(userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }


        //    return Ok(result.Data);
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            var result = await _profileService.SearchUsersAsync(query);
            return Ok(Result<PageResult<ProfileDto>>.Success(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _profileService.GetAllUsersAsync();
            return Ok(Result<PageResult<ProfileDto>>.Success(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Customers/Search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string query)
        {
            var result = await _profileService.SearchCustomersAsync(query);
            return Ok(Result<PageResult<ProfileDto>>.Success(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Customers/All")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _profileService.GetCustomersAsync();
            return Ok(Result<PageResult<ProfileDto>>.Success(result));
        }
    }
}
