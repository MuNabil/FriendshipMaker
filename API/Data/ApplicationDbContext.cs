using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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


            // To store all dates in db as Utc format
            builder.ApplyUtcDateTimeConverter();
        }
    }

    // For Utc time 
    public static class UtcDateAnnotation
    {
        private const String IsUtcAnnotation = "IsUtc";
        private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
          new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
          new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
          builder.HasAnnotation(IsUtcAnnotation, isUtc);

        public static Boolean IsUtc(this IMutableProperty property) =>
          ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

        /// <summary>
        /// Make sure this is called after configuring all your entities.
        /// </summary>
        public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (!property.IsUtc())
                    {
                        continue;
                    }

                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(UtcConverter);
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(UtcNullableConverter);
                    }
                }
            }
        }
    }
}