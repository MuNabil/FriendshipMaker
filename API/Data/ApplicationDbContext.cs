namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add the PK for Likes table as a (composite key)
            builder.Entity<UserLike>()
              .HasKey(x => new { x.SourceUserId, x.LikedUserId });

            // Add the ralationship of following
            builder.Entity<UserLike>()
              .HasOne(userLike => userLike.SourceUser)
              .WithMany(appUser => appUser.LikedUsers)
              .HasForeignKey(userLike => userLike.SourceUserId)
              .OnDelete(DeleteBehavior.Cascade);

            // Add the relationship of followers
            builder.Entity<UserLike>()
              .HasOne(userLike => userLike.LikedUser)
              .WithMany(appUser => appUser.LikedByUsers)
              .HasForeignKey(userLike => userLike.LikedUserId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}