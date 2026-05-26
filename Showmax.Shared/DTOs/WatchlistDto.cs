namespace Showmax.Shared.DTOs
{
    public class WatchlistDto
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int ReleaseYear { get; set; }
        public string Rating { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }

    public class AddToWatchlistDto
    {
        public int ContentId { get; set; }
        public int ProfileId { get; set; }
    }
}