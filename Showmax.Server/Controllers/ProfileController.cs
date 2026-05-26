using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showmax.Server.Data;
using Showmax.Server.Services;
using Showmax.Shared.DTOs;
using System.Security.Claims;
using AppUser = Showmax.Shared.Models.User;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;

        public ProfileController(AppDbContext context,
            UserManager<AppUser> userManager,
            CloudinaryService cloudinaryService)
        {
            _context = context;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }

        // GET current user profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            var watchlistCount = profile != null
                ? await _context.Watchlists.CountAsync(w => w.ProfileId == profile.Id)
                : 0;

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt,
                SubscriptionPlan = subscription?.Plan ?? "No Plan",
                IsSubscriptionActive = subscription?.IsActive ?? false,
                WatchlistCount = watchlistCount
            });
        }

        // PUT update profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.AvatarUrl))
                user.AvatarUrl = dto.AvatarUrl;

            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Profile updated successfully." });
        }

        // POST upload avatar
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file provided." });

            var result = await _cloudinaryService.UploadImageAsync(file);
            if (!result.IsSuccess)
                return BadRequest(new { Message = result.Message });

            user.AvatarUrl = result.Url;
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Avatar updated.", Url = result.Url });
        }

        // POST change password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            var result = await _userManager.ChangePasswordAsync(
                user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Message = errors });
            }

            return Ok(new { Message = "Password changed successfully." });
        }
    }
}