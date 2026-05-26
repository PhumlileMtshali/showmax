using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showmax.Server.Data;
using Showmax.Server.Services;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly AppDbContext _context;

        public UploadController(CloudinaryService cloudinaryService, AppDbContext context)
        {
            _cloudinaryService = cloudinaryService;
            _context = context;
        }

        // POST upload thumbnail
        [HttpPost("thumbnail")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file provided." });

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(file.ContentType))
                return BadRequest(new { Message = "Only JPEG, PNG and WebP images are allowed." });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { Message = "File size must be under 5MB." });

            var result = await _cloudinaryService.UploadImageAsync(file);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.Message });

            return Ok(result);
        }

        // POST upload video
        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file provided." });

            var allowed = new[] { "video/mp4", "video/mpeg", "video/quicktime" };
            if (!allowed.Contains(file.ContentType))
                return BadRequest(new { Message = "Only MP4, MPEG and MOV videos are allowed." });

            if (file.Length > 500 * 1024 * 1024)
                return BadRequest(new { Message = "File size must be under 500MB." });

            var result = await _cloudinaryService.UploadVideoAsync(file);

            if (!result.IsSuccess)
                return BadRequest(new { Message = result.Message });

            return Ok(result);
        }

        // POST update content thumbnail
        [HttpPost("update-thumbnail/{contentId}")]
        public async Task<IActionResult> UpdateContentThumbnail(int contentId, IFormFile file)
        {
            var content = await _context.Contents.FindAsync(contentId);
            if (content == null)
                return NotFound(new { Message = "Content not found." });

            var result = await _cloudinaryService.UploadImageAsync(file);
            if (!result.IsSuccess)
                return BadRequest(new { Message = result.Message });

            content.ThumbnailUrl = result.Url;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Thumbnail updated.", Url = result.Url });
        }

        // POST update content video
        [HttpPost("update-video/{contentId}")]
        public async Task<IActionResult> UpdateContentVideo(int contentId, IFormFile file)
        {
            var content = await _context.Contents.FindAsync(contentId);
            if (content == null)
                return NotFound(new { Message = "Content not found." });

            var result = await _cloudinaryService.UploadVideoAsync(file);
            if (!result.IsSuccess)
                return BadRequest(new { Message = result.Message });

            content.VideoUrl = result.Url;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Video updated.", Url = result.Url });
        }
    }
}