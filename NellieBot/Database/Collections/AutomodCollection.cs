using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using NellieBot.Database.Entities;

namespace NellieBot.Database.Collections
{
  static class AutomodCollection
  {
    public static async Task<AutomodData> GetAutomodRule(int id)
    {
      using var db = new DatabaseContext();
      return await db.AutomodRules.FirstAsync(x => x.Id == id);
    }

    public static async Task<List<AutomodData>> GetAllAutomodRules()
    {
      using var db = new DatabaseContext();
      return await db.AutomodRules.ToListAsync();
    }

    public static async Task<int> GetAutomodRuleCount()
    {
      using var db = new DatabaseContext();
      return await db.AutomodRules.CountAsync();
    }

    public static async Task AddAutomodRule(string label, List<string> words, List<string> regexes, string alert)
    {
      using var db = new DatabaseContext();
      var rule = new AutomodData()
      {
        Label = label,
        Words = words,
        Regexes = regexes,
        Alert = alert
      };

      await db.AutomodRules.AddAsync(rule);
      await db.SaveChangesAsync();
      await Program.DiscordConfig.RefreshAutomodRules();
    }

    public static async Task EditAutomodRule(int id, string label, List<string> words, List<string> regexes, string alert)
    {
      using var db = new DatabaseContext();
      var ruleToEdit = await db.AutomodRules.SingleAsync(x => x.Id == id);
      ruleToEdit.Label = label;
      ruleToEdit.Words = words;
      ruleToEdit.Regexes = regexes;
      ruleToEdit.Alert = alert;

      db.AutomodRules.Update(ruleToEdit);
      await db.SaveChangesAsync();
      await Program.DiscordConfig.RefreshAutomodRules();
    }

    public static async Task RemoveAutomodRule(int id)
    {
      using var db = new DatabaseContext();
      var ruleToDelete = await db.AutomodRules.SingleAsync(x => x.Id == id);

      db.AutomodRules.Remove(ruleToDelete);
      await db.SaveChangesAsync();
      await Program.DiscordConfig.RefreshAutomodRules();
    }
  }
}
