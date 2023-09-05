using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings { get; set; }

        public DatabaseContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlite($"Data Source=C:\\Users\\Joseph\\Desktop\\Heartstopper\\NellieBot\\NellieBot\\main_bot.db");

    }
}
