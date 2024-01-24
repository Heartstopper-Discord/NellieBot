using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using NellieBot.Extensions;
using NellieBot.Helper;
using System.Text.RegularExpressions;

namespace NellieBot.Events
{
  public class UserEvents
  {
    protected static async Task AutomodHandler(string message, DiscordChannel c, DiscordMember m) {
      bool detected = false;

      LogBuilder log = new LogBuilder(LogType.AutoModRule).WithEventEmbed(c, m);

      DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
      {
        Title = "Ableist Terms Detected"
      };
      foreach (var entry in Program.DiscordConfig.AutomodRules)
      {
        var matches = Regex.Matches(message, entry.Key, RegexOptions.IgnoreCase);
        if (matches.Count != 0) {
          detected = true;
          embedBuilder.AddField(entry.Value, string.Join(", ", matches.Select(x => x.Value)).TrimForEmbed());
          log = log.WithField(entry.Value, string.Join(", ", matches.Select(x => x.Value)).TrimForEmbed());
        }
      }
      if (!detected) return;

      try {
        await m.SendMessageAsync(embedBuilder);
        log = log.WithField("User notified with direct message.", "");
      } catch (UnauthorizedException) {
        log = log.WithField("Failed to notify user with direct message.", "");
      }
      finally {
        await log.Send();
      }
    }

    public static async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
      if (e.Author.IsBot || e.Channel.IsPrivate) return;
      await AutomodHandler(e.Message.Content, e.Channel, (DiscordMember)e.Author);
    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdateEventArgs e)
    {
      if (e.Author.IsCurrent || e.Message.WebhookMessage) return;
      await AutomodHandler(e.Message.Content, e.Channel, (DiscordMember)e.Author);

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
