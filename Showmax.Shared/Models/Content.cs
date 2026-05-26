namespace Showmax.Shared.Models
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Movie" or "Series"
        public string Genre { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string Rating { get; set; } = string.Empty; // e.g. PG, 16+
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
        public ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
        public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    }
}