using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using NellieBot.Helper;

namespace NellieBot.Events
{
  public class GuildEvents
  {
    public static async Task ThreadCreated(DiscordClient _, ThreadCreateEventArgs e)
    {
      await e.Thread.JoinThreadAsync();
    }
  }
}
