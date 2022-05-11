using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Database
{
    public class StockChatterContext : IdentityDbContext<UserDAO, IdentityRole<Guid>, Guid>
    {
        public StockChatterContext(DbContextOptions options)
            : base(options) { }

        public DbSet<UserDAO> Users { get; set; }
        public DbSet<MessageDAO> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
