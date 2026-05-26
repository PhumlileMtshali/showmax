namespace Showmax.Shared.Models
{
    public class WatchHistory
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;
        public int PositionSeconds { get; set; }
        public DateTime LastWatched { get; set; } = DateTime.UtcNow;
    }
}