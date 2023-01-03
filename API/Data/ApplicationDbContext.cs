namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
     IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region M-N relationship between Users and Roles

            builder.Entity<ApplicationUser>()
              .HasMany(appUser => appUser.UserRoles)
              .WithOne(userRole => userRole.User)
              .HasForeignKey(appUser => appUser.UserId)
              .IsRequired();

            builder.Entity<ApplicationRole>()
              .HasMany(appRole => appRole.UserRoles)
              .WithOne(userRole => userRole.Role)
              .HasForeignKey(appRole => appRole.RoleId)
              .IsRequired();
            #endregion

            #region Like relationship

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

            #endregion

            #region Message relationship

            builder.Entity<Message>()
              .HasOne(message => message.Sender)
              .WithMany(user => user.MessagesSent)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
              .HasOne(message => message.Recipient)
              .WithMany(user => user.MessagesReceived)
              .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}