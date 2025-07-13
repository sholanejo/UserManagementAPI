using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserManagementAPI.Application.DTOs;
using UserManagementAPI.Application.Interfaces;
using UserManagementAPI.Domain.Enums;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {

        /// <summary>
        /// Get all users with optional search and pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <param name="search">Search term for filtering users</param>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var users = await userService.GetAllAsync(page, pageSize, search);
            var totalCount = await userService.GetTotalCountAsync(search);

            var response = new
            {
                Data = users,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await userService.GetByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="dto">User creation data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="dto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var user = await userService.UpdateAsync(id, dto);
            return Ok(user);
        }

        /// <summary>
        /// Delete a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Restore a soft-deleted user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Restored user</returns>
        [HttpPost("{id}/restore")]
        [EnableRateLimiting("fixed")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> RestoreUser(Guid id)
        {
            var user = await userService.RestoreAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await userService.GetByIdAsync(userId);
            return Ok(user);
        }
    }
}
