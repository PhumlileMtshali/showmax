namespace Showmax.Shared.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string Plan { get; set; } = string.Empty; // "Basic", "Standard", "Premium"
        public string? StripeSubscriptionId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? RenewalDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}