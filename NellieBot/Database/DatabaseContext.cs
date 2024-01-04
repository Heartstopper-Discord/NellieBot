using Microsoft.EntityFrameworkCore;
using NellieBot.Database.Entities;

namespace NellieBot.Database
{
  public class DatabaseContext : DbContext
  {
    public DbSet<WarnData> Warns { get; set; }
    public DbSet<TimeoutData> Timeouts { get; set; }

    public DatabaseContext()
    {
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
      Database.EnsureCreated();
    }
    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseSqlite($"Data Source=C:\\Users\\Joseph\\Desktop\\Heartstopper\\NellieBot\\NellieBot\\main_bot.db");
    */
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseNpgsql($"Host={Program.BotConfig.DBHost};Database=bot_db;Username={Program.BotConfig.DBUsername};Password={Program.BotConfig.DBPassword}");
  }
}
