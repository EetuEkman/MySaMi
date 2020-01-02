using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySaMi.Models;
using System;

namespace MySaMi.Data
{
    public class MySaMiDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<StatisticsModel> Statistics { get; set; }
        public MySaMiDbContext(DbContextOptions<MySaMiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Keys)
                .WithOne(k => k.ApplicationUser)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Queries)
                .WithOne(q => q.ApplicationUser)
                .IsRequired();

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "User", NormalizedName = "USER", ConcurrencyStamp = Guid.NewGuid().ToString() }, 
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString() }
            );

            builder.Entity<StatisticsModel>().HasData(new StatisticsModel { Id = 1, QueryCount = 0, MeasurementCount = 0 });
        }

        public DbSet<SaMiKeyModel> SaMiKeyModel { get; set; }
    }
}
