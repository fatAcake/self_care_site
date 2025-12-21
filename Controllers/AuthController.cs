using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Self_care.Data;
using Self_care.DTOs;
using Self_care.Models;
using Self_care.Services;

namespace Self_care.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SelfCareDB _db;
        private readonly TokenService _tokenService;

        public AuthController(SelfCareDB db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.email && !u.Deleted);

            if (existingUser != null)
            {
                return BadRequest(new { error = "Email is already registered." });
            }

            var user = new User
            {
                Nickname = dto.nickname,
                Email = dto.email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                RegistrationDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Timezone = dto.timezone,
                RoleId = 1,
                Deleted = false
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { user_id = user.UserId }, new
            {
                message = "User registered successfully.",
                user_id = user.UserId
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.email && !u.Deleted);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.password, user.Password))
                return Unauthorized("Invalid credentials");

            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                access_token = token,
                user_id = user.UserId,
                nickname = user.Nickname,
                email = user.Email,
                timezone = user.Timezone
            });
        }
    }
}
