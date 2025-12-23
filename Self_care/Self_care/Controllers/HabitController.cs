using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Self_care.Data;
using Self_care.DTOs;
using Self_care.Models;
using System.Security.Claims;
using System;

namespace Self_care.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HabitController : ControllerBase
    {
        private readonly SelfCareDB _db;

        public HabitController(SelfCareDB db)
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

        private DateTime NormalizeDate(DateTime date)
        {
            var d = date.Date;
            return DateTime.SpecifyKind(d, DateTimeKind.Unspecified);
        }

        private DateTime UnspecifiedNow()
        {
            return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllHabits()
        {
            try
            {
                var habits = await _db.Habits
                    .Where(h => !h.Deleted)
                    .Select(h => new HabitResponseDto
                    {
                        HabitsId = h.HabitsId,
                        Name = h.Name,
                        Description = h.Description
                    })
                    .ToListAsync();

                return Ok(habits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("user-habits")]
        public async Task<IActionResult> GetUserHabits([FromQuery] DateTime? date = null)
        {
            try
            {
                var userId = GetUserId();
                var targetDate = NormalizeDate(date ?? DateTime.UtcNow);
                var targetDateEnd = targetDate.AddDays(1);

                var userHabits = await _db.UserHabits
                    .Include(uh => uh.Habit)
                    .Where(uh => uh.UserId == userId 
                        && !uh.Deleted 
                        && uh.ExecuteDate >= targetDate 
                        && uh.ExecuteDate < targetDateEnd)
                    .Select(uh => new UserHabitResponseDto
                    {
                        UsHabitId = uh.UsHabitId,
                        HabitId = uh.HabitId,
                        HabitName = uh.Habit.Name,
                        IsMarked = uh.IsMarked,
                        ExecuteDate = uh.ExecuteDate,
                        Status = uh.Status
                    })
                    .ToListAsync();

                return Ok(userHabits);
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

        [HttpGet("progress")]
        public async Task<IActionResult> GetProgress([FromQuery] DateTime? date = null)
        {
            try
            {
                var userId = GetUserId();
                var targetDate = NormalizeDate(date ?? DateTime.UtcNow);
                var targetDateEnd = targetDate.AddDays(1);

                var total = await _db.UserHabits
                    .Where(uh => uh.UserId == userId 
                        && !uh.Deleted 
                        && uh.ExecuteDate >= targetDate 
                        && uh.ExecuteDate < targetDateEnd)
                    .CountAsync();

                var completed = await _db.UserHabits
                    .Where(uh => uh.UserId == userId 
                        && !uh.Deleted 
                        && uh.ExecuteDate >= targetDate 
                        && uh.ExecuteDate < targetDateEnd
                        && uh.IsMarked)
                    .CountAsync();

                return Ok(new ProgressResponseDto
                {
                    Completed = completed,
                    Total = total
                });
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

        [HttpPost("add-to-user")]
        public async Task<IActionResult> AddHabitToUser([FromBody] AddHabitToUserDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { error = "Payload is empty" });
                }
                if (dto.HabitId <= 0)
                {
                    return BadRequest(new { error = "HabitId must be > 0" });
                }
                if (dto.ExecuteDate == default)
                {
                    return BadRequest(new { error = "ExecuteDate is required" });
                }

                var userId = GetUserId();
                var habit = await _db.Habits
                    .FirstOrDefaultAsync(h => h.HabitsId == dto.HabitId && !h.Deleted);

                if (habit == null)
                {
                    return NotFound(new { error = "Habit not found" });
                }

                var executeDate = NormalizeDate(dto.ExecuteDate);
                var executeDateEnd = executeDate.AddDays(1);


                var existing = await _db.UserHabits
                    .FirstOrDefaultAsync(uh => uh.UserId == userId 
                        && uh.HabitId == dto.HabitId 
                        && uh.ExecuteDate >= executeDate 
                        && uh.ExecuteDate < executeDateEnd
                        && !uh.Deleted);

                if (existing != null)
                {
                    return BadRequest(new { error = "Habit already added for this date" });
                }

                var userHabit = new UserHabit
                {
                    UserId = userId,
                    HabitId = dto.HabitId,
                    ExecuteDate = executeDate,
                    IsMarked = false,
                    Status = "active",
                    Frequency = "daily",
                    CreatedAt = UnspecifiedNow(),
                    Deleted = false
                };

                _db.UserHabits.Add(userHabit);
                await _db.SaveChangesAsync();

                return Ok(new UserHabitResponseDto
                {
                    UsHabitId = userHabit.UsHabitId,
                    HabitId = userHabit.HabitId,
                    HabitName = habit.Name,
                    IsMarked = userHabit.IsMarked,
                    ExecuteDate = userHabit.ExecuteDate,
                    Status = userHabit.Status
                });
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateHabit([FromBody] CreateHabitDto dto)
        {
            try
            {
                var habit = new Habit
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedAt = UnspecifiedNow(),
                    Deleted = false
                };

                _db.Habits.Add(habit);
                await _db.SaveChangesAsync();

                return Ok(new HabitResponseDto
                {
                    HabitsId = habit.HabitsId,
                    Name = habit.Name,
                    Description = habit.Description
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPatch("toggle/{userHabitId}")]
        public async Task<IActionResult> ToggleHabit(int userHabitId)
        {
            try
            {
                var userId = GetUserId();

                var userHabit = await _db.UserHabits
                    .Include(uh => uh.Habit)
                    .FirstOrDefaultAsync(uh => uh.UsHabitId == userHabitId 
                        && uh.UserId == userId 
                        && !uh.Deleted);

                if (userHabit == null)
                {
                    return NotFound(new { error = "User habit not found" });
                }

                userHabit.IsMarked = !userHabit.IsMarked;
                userHabit.UpdatedAt = UnspecifiedNow();

                await _db.SaveChangesAsync();

                return Ok(new UserHabitResponseDto
                {
                    UsHabitId = userHabit.UsHabitId,
                    HabitId = userHabit.HabitId,
                    HabitName = userHabit.Habit.Name,
                    IsMarked = userHabit.IsMarked,
                    ExecuteDate = userHabit.ExecuteDate,
                    Status = userHabit.Status
                });
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

        [HttpDelete("user-habits/{userHabitId}")]
        public async Task<IActionResult> DeleteUserHabit(int userHabitId)
        {
            try
            {
                var userId = GetUserId();

                var userHabit = await _db.UserHabits
                    .FirstOrDefaultAsync(uh => uh.UsHabitId == userHabitId
                        && uh.UserId == userId
                        && !uh.Deleted);

                if (userHabit == null)
                {
                    return NotFound(new { error = "User habit not found" });
                }

                userHabit.Deleted = true;
                userHabit.DeletedAt = UnspecifiedNow();
                await _db.SaveChangesAsync();

                return NoContent();
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

