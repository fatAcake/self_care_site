using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Self_care.Data;
using Self_care.DTOs;
using Self_care.Models;
using System.Security.Claims;

namespace Self_care.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly SelfCareDB _db;

        public SessionsController(SelfCareDB db)
        {
            _db = db;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }

        // Создание сессии для после использования пользователем приложения
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] SessionCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var userId = GetUserId();

                var session = new Session
                {
                    UserId = userId,
                    Type = dto.Type,
                    Duration = (long)dto.Duration,
                    Completed = dto.Completed ?? true,
                    Notes = dto.Notes,
                    StartedAt = DateTime.SpecifyKind(dto.StartedAt, DateTimeKind.Unspecified),
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    UpdatedAt = null,
                    DeletedAt = null,
                    Deleted = false
                };

                _db.Sessions.Add(session);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateSession), new { id = session.SessionId }, new { session_id = session.SessionId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}

