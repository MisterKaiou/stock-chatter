﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Database
{
    public class StockChattyContext : IdentityDbContext<UserDAO, IdentityRole<Guid>, Guid>
    {
        public StockChattyContext(DbContextOptions options)
            : base(options) { }

        public DbSet<UserDAO> Users { get; set; }
    }
}
