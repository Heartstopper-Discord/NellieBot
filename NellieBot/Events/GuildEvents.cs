using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NellieBot.Helper;

namespace NellieBot.Events
{
  public class GuildEvents
  {
    public static async Task ThreadCreated(DiscordClient _, ThreadCreatedEventArgs e)
    {
      await e.Thread.JoinThreadAsync();
      var m = await e.Thread.SendMessageAsync(Program.DiscordConfig.Moderator.Mention);
      await m.DeleteAsync();
    }
  }
}
