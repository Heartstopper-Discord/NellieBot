﻿using DSharpPlus.Entities;
using NellieBot.Database.Entities;

namespace NellieBot.Database.Collections
{
    static class TimeoutCollection
    {
        public static TimeoutData getCurrentTimeout(DiscordMember m)
        {
            using var db = new DatabaseContext();
            DateTime now = DateTime.Now
            return db.Timeouts.Where(x => x.UserId == m.Id).Where(x => x.Until > now).ToList(); //No idea if this works... dead brain coding time
        }

        public static List<TimeoutData> GetTimeouts(DiscordMember m)
        {
            using var db = new DatabaseContext();
            return db.Timeouts.Where(x => x.UserId == m.Id).ToList();
        }

        public static int GetTimeoutCount(DiscordMember m)
        {
            using var db = new DatabaseContext();
            return db.Timeout.Count(x => x.UserId == m.Id);
        }

        public static async Task AddTimeout(DiscordMember m, string reason, TimeSpan timeoutLength)
        {
            using var db = new DatabaseContext();

            var timeout = new TimeoutData()
            {
                Id = GetTimeoutCount(m),
                UserId = m.Id,
                Reason = reason,
                DateTime = DateTime.Now,
                Until = DateTime.Now.Add(timeoutLength)
            };

            await db.Timeouts.AddAsync(warn);
            await db.SaveChangesAsync();
        }

        public static void RemoveTimeout(DiscordMember m, int id)
        {
            using var db = new DatabaseContext();
            var warnToDelete = db.Warns.First(x => x.Id == id && x.UserId == m.Id);
            warnToDelete.Until = DateTime.Now;
            
            db.SaveChanges();
        }
    }
}
