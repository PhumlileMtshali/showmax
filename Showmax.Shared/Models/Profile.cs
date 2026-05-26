namespace Showmax.Shared.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsKids { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
        public ICollection<Watchlist> Watchlist { get; set; } = new List<Watchlist>();
    }
}