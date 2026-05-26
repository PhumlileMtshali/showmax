namespace Showmax.Shared.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int DurationMinutes { get; set; }
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;
    }
}