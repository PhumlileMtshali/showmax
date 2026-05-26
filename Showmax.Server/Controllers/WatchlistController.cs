using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showmax.Server.Data;
using Showmax.Shared.DTOs;
using Showmax.Shared.Models;
using System.Security.Claims;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WatchlistController(AppDbContext context)
        {
            _context = context;
        }

        // GET user's watchlist
        [HttpGet]
        public async Task<IActionResult> GetWatchlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get first profile for the user
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return Ok(new List<WatchlistDto>());

            var watchlist = await _context.Watchlists
                .Where(w => w.ProfileId == profile.Id)
                .Include(w => w.Content)
                .OrderByDescending(w => w.AddedAt)
                .Select(w => new WatchlistDto
                {
                    Id = w.Id,
                    ContentId = w.ContentId,
                    Title = w.Content.Title,
                    Type = w.Content.Type,
                    Genre = w.Content.Genre,
                    ThumbnailUrl = w.Content.ThumbnailUrl,
                    ReleaseYear = w.Content.ReleaseYear,
                    Rating = w.Content.Rating,
                    AddedAt = w.AddedAt
                })
                .ToListAsync();

            return Ok(watchlist);
        }

        // POST add to watchlist
        [HttpPost("{contentId}")]
        public async Task<IActionResult> AddToWatchlist(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get or create profile
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var user = await _context.Users.FindAsync(userId);
                profile = new Profile
                {
                    UserId = userId!,
                    Name = user?.FullName ?? "My Profile",
                    IsKids = false
                };
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            // Check if already in watchlist
            var existing = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.ProfileId == profile.Id && w.ContentId == contentId);

            if (existing != null)
                return BadRequest(new { Message = "Already in watchlist." });

            var watchlistItem = new Watchlist
            {
                ProfileId = profile.Id,
                ContentId = contentId,
                AddedAt = DateTime.UtcNow
            };

            _context.Watchlists.Add(watchlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Added to watchlist." });
        }

        // DELETE remove from watchlist
        [HttpDelete("{contentId}")]
        public async Task<IActionResult> RemoveFromWatchlist(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound(new { Message = "Profile not found." });

            var watchlistItem = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.ProfileId == profile.Id && w.ContentId == contentId);

            if (watchlistItem == null)
                return NotFound(new { Message = "Item not in watchlist." });

            _context.Watchlists.Remove(watchlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Removed from watchlist." });
        }

        // GET check if content is in watchlist
        [HttpGet("check/{contentId}")]
        public async Task<IActionResult> CheckWatchlist(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return Ok(new { IsInWatchlist = false });

            var exists = await _context.Watchlists
                .AnyAsync(w => w.ProfileId == profile.Id && w.ContentId == contentId);

            return Ok(new { IsInWatchlist = exists });
        }
    }
}