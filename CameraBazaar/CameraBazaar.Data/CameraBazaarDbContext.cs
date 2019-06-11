namespace CameraBazaar.Data
{
    using CameraBazaar.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class CameraBazaarDbContext : IdentityDbContext<User> // Identity => App User
    {
        public CameraBazaarDbContext(DbContextOptions<CameraBazaarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Camera> Cameras { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
               .Entity<Camera>()
               .HasOne(c => c.User)
               .WithMany(u => u.Cameras)
               .HasForeignKey(c => c.UserId);

            builder
                .Entity<Camera>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18, 2)"); // decimal
        }
    }
}
