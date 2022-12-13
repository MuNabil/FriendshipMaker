using System;
using System.Collections.Generic;
namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}