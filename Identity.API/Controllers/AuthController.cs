using MyShop.CORE.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using MyShop.CORE.Interfaces;
using MyShop.Core.Dtos;
using Identity.Core.Interfaces;

namespace MyShop.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProfileService _profileService;

        public AuthController(IAuthService authService, IProfileService userService)
        {
            _authService = authService;
            _profileService = userService;
        }
        #region Authentication Endpoints
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        
        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto dto)
        {
            var result = await _authService.ConfirmEmailAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        
        [AllowAnonymous]
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto dto)
        {
            var result = await _authService.ResendEmailConfirmationAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }

        
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }
        
        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _authService.GoogleLoginAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);

            return Ok(result.Data);
        }
        #endregion
        

        
        



    }
}
