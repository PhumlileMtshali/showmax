using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showmax.Server.Data;
using Showmax.Shared.DTOs;
using Showmax.Shared.Models;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContentController(AppDbContext context)
        {
            _context = context;
        }

        // GET all content
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var content = await _context.Contents
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

        // GET single content by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var content = await _context.Contents
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
                return NotFound(new { Message = "Content not found." });

            return Ok(new ContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Synopsis = content.Synopsis,
                Type = content.Type,
                Genre = content.Genre,
                Language = content.Language,
                ReleaseYear = content.ReleaseYear,
                ThumbnailUrl = content.ThumbnailUrl,
                VideoUrl = content.VideoUrl,
                Rating = content.Rating
            });
        }

        // GET content by type (Movie or Series)
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type)
        {
            var content = await _context.Contents
                .Where(c => c.Type.ToLower() == type.ToLower())
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

        // GET content by genre
        [HttpGet("genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            var content = await _context.Contents
                .Where(c => c.Genre.ToLower() == genre.ToLower())
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

        // GET search content by title
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var content = await _context.Contents
                .Where(c => c.Title.ToLower().Contains(query.ToLower()) ||
                            c.Genre.ToLower().Contains(query.ToLower()))
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

        // POST create content (Admin only)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateContentDto dto)
        {
            var content = new Content
            {
                Title = dto.Title,
                Synopsis = dto.Synopsis,
                Type = dto.Type,
                Genre = dto.Genre,
                Language = dto.Language,
                ReleaseYear = dto.ReleaseYear,
                ThumbnailUrl = dto.ThumbnailUrl,
                VideoUrl = dto.VideoUrl,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Content created successfully.", Id = content.Id });
        }

        // PUT update content (Admin only)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateContentDto dto)
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
            content.ThumbnailUrl = dto.ThumbnailUrl;
            content.VideoUrl = dto.VideoUrl;
            content.Rating = dto.Rating;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Content updated successfully." });
        }

        // DELETE content (Admin only)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null)
                return NotFound(new { Message = "Content not found." });

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Content deleted successfully." });
        }
    }
}