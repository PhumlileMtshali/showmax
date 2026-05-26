namespace Showmax.Shared.Models
{
    public class Watchlist
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}