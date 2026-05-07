using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Profile;

using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using MyShop.Application.Interfaces;
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
            return StatusCode(result.Status, result);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.GetUserByIdAsync(userId);
            return StatusCode(result.Status, result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.UpdateProfileAsync(userId, dto);
            return StatusCode(result.Status, result);
        }
        
        [Authorize]
        [HttpPost("Upload-Image")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.UploadProfilePictureAsync(userId, file);
            return StatusCode(result.Status, result);
        }
        [Authorize]
        [HttpDelete("Delete-Image")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.DeleteProfilePictureAsync(userId);
            return StatusCode(result.Status, result);
        }
        [Authorize]
        [HttpDelete("Delete-Account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _profileService.DeleteAccountAsync(userId);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            var result = await _profileService.SearchUsersAsync(query);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _profileService.GetAllUsersAsync();
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Customers/Search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string query)
        {
            var result = await _profileService.SearchCustomersAsync(query);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Customers/All")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _profileService.GetCustomersAsync();
            return StatusCode(result.Status, result);
        }
    }
}
