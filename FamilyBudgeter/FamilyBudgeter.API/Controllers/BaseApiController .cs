using FamilyBudgeter.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FamilyBudgeter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Користувач не автентифікований");
            }
            return userId;
        }

        protected IActionResult HandleException(Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            if (ex is UnauthorizedAccessException)
                return Unauthorized(new { message = ex.Message });

            if (ex is InvalidOperationException)
                return BadRequest(new { message = ex.Message });

            if (ex is ArgumentException)
                return BadRequest(new { message = ex.Message });

            // Логування помилки тут (в реальному проекті)
            return StatusCode(500, new { message = "Виникла внутрішня помилка сервера" });
        }
    }
}
