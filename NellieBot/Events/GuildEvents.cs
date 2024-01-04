using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using NellieBot.Database;
using NellieBot.Extensions;

namespace NellieBot.Events
{
  public class GuildEvents
  {
    public static async Task GuildAvailable(DiscordClient _, GuildCreateEventArgs e)
    {
      if (Program.BotConfig.GuildId == e.Guild.Id)
      {
        Program.DiscordConfig = new DiscordConfig(e.Guild, Program.BotConfig);
        await Task.CompletedTask;
      }
    }
  }
}
