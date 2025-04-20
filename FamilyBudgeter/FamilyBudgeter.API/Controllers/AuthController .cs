using FamilyBudgeter.API.BLL.DTOs.AuthDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registrationDto);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                if (!result.Success)
                {
                    return Unauthorized(new { message = result.Message });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto.Token, refreshTokenDto.RefreshToken);
                if (!result.Success)
                {
                    return Unauthorized(new { message = result.Message });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _authService.GetCurrentUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId != userDto.Id)
                {
                    return BadRequest(new { message = "Ви можете оновлювати тільки свої дані" });
                }
                var result = await _authService.UpdateUserAsync(userId, userDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }

    public class RefreshTokenDto
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }

    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
