using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using NellieBot.Database.Entities;

namespace NellieBot.Database.Collections
{
  static class WarnCollection
  {
    public static async Task<List<WarnData>> GetWarns(DiscordMember m)
    {
      using var db = new DatabaseContext();
      return await db.Warns.Where(x => x.UserId == m.Id).ToListAsync();
    }

    public static async Task<int> GetWarnCount(DiscordMember m)
    {
      using var db = new DatabaseContext();
      return await db.Warns.Where(x => x.UserId == m.Id).MaxAsync(x => x.Id);
    }

    public static async Task AddWarn(DiscordMember m, string reason, string note)
    {
      using var db = new DatabaseContext();
      var warn = new WarnData()
      {
        Id = await GetWarnCount(m) + 1,
        UserId = m.Id,
        Reason = reason,
        Note = note,
        DateTime = DateTime.Now
      };

      await db.Warns.AddAsync(warn);
      await db.SaveChangesAsync();
    }

    public static async Task RemoveWarn(DiscordMember m, int id)
    {
      using var db = new DatabaseContext();
      var warnToDelete = db.Warns.First(x => x.Id == id && x.UserId == m.Id);
      db.Warns.Remove(warnToDelete);

      await db.SaveChangesAsync();
    }
  }
}
