using Microsoft.AspNetCore.Identity;

namespace Showmax.Shared.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
        public Subscription? Subscription { get; set; }
    }
}