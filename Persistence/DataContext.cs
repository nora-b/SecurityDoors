using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            SetIdentityTableNames(builder);
        }

        private static void SetIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(name: "Users");
            modelBuilder.Entity<Role>().ToTable(name: "Roles");
        }
    }
}