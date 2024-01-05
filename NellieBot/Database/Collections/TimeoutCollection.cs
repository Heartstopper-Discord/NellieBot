using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using NellieBot.Database.Entities;

namespace NellieBot.Database.Collections
{
  static class TimeoutCollection
  {
    public static async Task<TimeoutData> GetCurrentTimeout(DiscordMember m)
    {
      using var db = new DatabaseContext();
      DateTime now = DateTime.Now;
      return await db.Timeouts.Where(x => x.UserId == m.Id).Where(x => x.Until > now).FirstAsync(); //No idea if this works... dead brain coding time
    }

    public static async Task<List<TimeoutData>> GetTimeouts(DiscordMember m)
    {
      using var db = new DatabaseContext();
      return await db.Timeouts.Where(x => x.UserId == m.Id).ToListAsync();
    }

    public static async Task<int> GetTimeoutCount(DiscordMember m)
    {
      using var db = new DatabaseContext();
      return await db.Timeouts.Where(x => x.UserId == m.Id).MaxAsync(x => x.Id);
    }

    public static async Task AddTimeout(DiscordMember m, string reason, TimeSpan timeoutLength)
    {
      using var db = new DatabaseContext();

      var timeout = new TimeoutData()
      {
        Id = await GetTimeoutCount(m) + 1,
        UserId = m.Id,
        Reason = reason,
        DateTime = DateTime.Now,
        Until = DateTime.Now.Add(timeoutLength)
      };

      await db.Timeouts.AddAsync(timeout);
      await db.SaveChangesAsync();
    }

    public static async Task RemoveTimeout(DiscordMember m, int id)
    {
      using var db = new DatabaseContext();
      var warnToDelete = db.Timeouts.First(x => x.Id == id && x.UserId == m.Id);
      warnToDelete.Until = DateTime.Now;

      await db.SaveChangesAsync();
    }
  }
}
