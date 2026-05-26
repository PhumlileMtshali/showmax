using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showmax.Server.Data;
using Showmax.Server.Services;
using Showmax.Shared.DTOs;
using Showmax.Shared.Models;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public AdminController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("content")]
        public async Task<IActionResult> GetAllContent()
        {
            var content = await _context.Contents
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ContentDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Synopsis = c.Synopsis,
                    Type = c.Type,
                    Genre = c.Genre,
                    Language = c.Language,
                    ReleaseYear = c.ReleaseYear,
                    ThumbnailUrl = c.ThumbnailUrl,
                    VideoUrl = c.VideoUrl,
                    Rating = c.Rating
                })
                .ToListAsync();

            return Ok(content);
        }

        [HttpPost("content")]
        public async Task<IActionResult> AddContent([FromForm] CreateContentDto dto,
            IFormFile? thumbnail, IFormFile? video)
        {
            var content = new Content
            {
                Title = dto.Title,
                Synopsis = dto.Synopsis,
                Type = dto.Type,
                Genre = dto.Genre,
                Language = dto.Language,
                ReleaseYear = dto.ReleaseYear,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };

            // Use URL if no file uploaded
            if (thumbnail != null)
            {
                var thumbResult = await _cloudinaryService.UploadImageAsync(thumbnail);
                if (thumbResult.IsSuccess)
                    content.ThumbnailUrl = thumbResult.Url;
            }
            else if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            {
                content.ThumbnailUrl = dto.ThumbnailUrl;
            }

            if (video != null)
            {
                var videoResult = await _cloudinaryService.UploadVideoAsync(video);
                if (videoResult.IsSuccess)
                    content.VideoUrl = videoResult.Url;
            }
            else if (!string.IsNullOrEmpty(dto.VideoUrl))
            {
                content.VideoUrl = dto.VideoUrl;
            }

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Content added successfully.", Id = content.Id });
        }

        [HttpPut("content/{id}")]
        public async Task<IActionResult> UpdateContent(int id, [FromForm] CreateContentDto dto,
            IFormFile? thumbnail, IFormFile? video)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null)
                return NotFound(new { Message = "Content not found." });

            content.Title = dto.Title;
            content.Synopsis = dto.Synopsis;
            content.Type = dto.Type;
            content.Genre = dto.Genre;
            content.Language = dto.Language;
            content.ReleaseYear = dto.ReleaseYear;
            content.Rating = dto.Rating;

            // Use URL if no file uploaded
            if (thumbnail != null)
            {
                var thumbResult = await _cloudinaryService.UploadImageAsync(thumbnail);
                if (thumbResult.IsSuccess)
                    content.ThumbnailUrl = thumbResult.Url;
            }
            else if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            {
                content.ThumbnailUrl = dto.ThumbnailUrl;
            }

            if (video != null)
            {
                var videoResult = await _cloudinaryService.UploadVideoAsync(video);
                if (videoResult.IsSuccess)
                    content.VideoUrl = videoResult.Url;
            }
            else if (!string.IsNullOrEmpty(dto.VideoUrl))
            {
                content.VideoUrl = dto.VideoUrl;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Content updated successfully." });
        }

        [HttpDelete("content/{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null)
                return NotFound(new { Message = "Content not found." });

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Content deleted successfully." });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalContent = await _context.Contents.CountAsync();
            var totalMovies = await _context.Contents.CountAsync(c => c.Type == "Movie");
            var totalSeries = await _context.Contents.CountAsync(c => c.Type == "Series");
            var totalUsers = await _context.Users.CountAsync();
            var totalSubscriptions = await _context.Subscriptions.CountAsync(s => s.IsActive);

            return Ok(new
            {
                TotalContent = totalContent,
                TotalMovies = totalMovies,
                TotalSeries = totalSeries,
                TotalUsers = totalUsers,
                TotalSubscriptions = totalSubscriptions
            });
        }
    }
}