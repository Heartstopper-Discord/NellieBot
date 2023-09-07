using DSharpPlus.Entities;
using NellieBot.Database.Entities;

namespace NellieBot.Database.Collections
{
    static class WarnCollection
    {
        public static List<WarnData> GetWarns(DiscordMember m)
        {
            using var db = new DatabaseContext();
            return db.Warns.Where(x => x.UserId == m.Id).ToList();
        }

        public static int GetWarnCount(DiscordMember m)
        {
            using var db = new DatabaseContext();
            return db.Warns.Count(x => x.UserId == m.Id);
        }

        public static async Task AddWarn(DiscordMember m, string reason, string note)
        {
            using var db = new DatabaseContext();

            var warn = new WarnData()
            {
                Id = GetWarnCount(m),
                UserId = m.Id,
                Reason = reason,
                Note = note,
                DateTime = DateTime.Now
            };

            await db.Warns.AddAsync(warn);
            await db.SaveChangesAsync();
        }

        public static void RemoveWarn(DiscordMember m, int id)
        {
            using var db = new DatabaseContext();
            var warnToDelete = db.Warns.First(x => x.Id == id && x.UserId == m.Id);
            db.Warns.Remove(warnToDelete);
            
            db.SaveChanges();
        }
    }
}
