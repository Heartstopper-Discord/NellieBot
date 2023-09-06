using Microsoft.EntityFrameworkCore;
using NellieBot.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<WarnData> Warns { get; set; }
        public DbSet<WarnData> Timeouts { get; set; }

        public DatabaseContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlite($"Data Source=C:\\Users\\Joseph\\Desktop\\Heartstopper\\NellieBot\\NellieBot\\main_bot.db");

    }
}
