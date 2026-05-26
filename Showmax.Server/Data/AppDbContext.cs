using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Showmax.Shared.Models;

// Use aliases to avoid conflict with Stripe.Subscription
using AppUser = Showmax.Shared.Models.User;
using AppSubscription = Showmax.Shared.Models.Subscription;

namespace Showmax.Server.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<AppSubscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(u => u.Subscription)
                .WithOne(s => s.User)
                .HasForeignKey<AppSubscription>(s => s.UserId);

            builder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(p => p.UserId);
        }
    }
}