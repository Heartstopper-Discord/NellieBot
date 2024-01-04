using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NellieBot.Extensions;
using NellieBot.Helper;
using System.Diagnostics;

namespace NellieBot.Events
{
  public class UserEvents
  {
    public static async Task GuildMemberAdded(DiscordClient _, GuildMemberAddEventArgs e)
    {
      DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
      {
        Title = "Test"
      }
      .AddField("Hello", "hi");

      await e.Member.SendMessageAsync(embedBuilder);
    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdateEventArgs e)
    {
      if (e.Author.IsCurrent || e.Message.WebhookMessage) return;

      await new LogBuilder(LogType.MessageUpdated)
        .WithEventEmbed(e.Channel, (DiscordMember)e.Message.Author)
        .WithField("Previous Contents", StringEx.DefaultIfNullOrEmpty(e.MessageBefore?.Content, "Failed to retrieve previous message contents."))
        .WithField("New Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve new message contents."))
        .WithAuthorAndAttachmentInfo(e.Message!)
        .Send();
    }

    public static async Task MessageDeleted(DiscordClient _, MessageDeleteEventArgs e)
    {
      if (e.Message.Author.IsCurrent || e.Message.WebhookMessage) return;

      await new LogBuilder(LogType.MessageDeleted)
        .WithEventEmbed(e.Channel, (DiscordMember)e.Message.Author)
        .WithField("Message Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve message contents."))
        .WithAuthorAndAttachmentInfo(e.Message!)
        .Send();
    }
  }
}
