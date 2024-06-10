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
    protected static async Task AutomodHandler(DiscordMessage message, DiscordChannel c, DiscordMember m) {
      bool detected = false;

      LogBuilder log = new LogBuilder(LogType.AutomodRuleBroken)
        .WithEventEmbed(c, m)
        .WithField("Link to message", message.JumpLink.ToString());

      DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
      {
        Title = "Ableist Terms Detected"
      };
      foreach (var entry in Program.DiscordConfig.AutomodRules)
      {
        var matches = Regex.Matches(message.Content, entry.Key, RegexOptions.IgnoreCase);
        if (matches.Count != 0) {
          detected = true;
          embedBuilder
            .AddField("Suggestion", entry.Value)
            .AddField("Words", string.Join(", ", matches.Select(x => x.Value).Distinct()));
          log
            .WithField("Suggestion", entry.Value)
            .WithField("Words", string.Join(", ", matches.Select(x => x.Value).Distinct()));
        }
      }
      if (!detected) return;

      try {
        await m.SendMessageAsync(embedBuilder);
        log.WithField("DM Success", "Yes");
      } catch (UnauthorizedException) {
        log.WithField("DM Success", "No");
      }
      finally {
        await log.Send();
      }
    }

    public static async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
      if (e.Author.IsBot || e.Channel.IsPrivate) return;
      await AutomodHandler(e.Message, e.Channel, (DiscordMember)e.Author);
    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdateEventArgs e)
    {
      if (e.Author.IsCurrent || e.Message.WebhookMessage) return;
      await AutomodHandler(e.Message, e.Channel, (DiscordMember)e.Author);
      // await new LogBuilder(LogType.MessageUpdated)
      //   .WithEventEmbed(e.Channel, (DiscordMember)e.Message.Author)
      //   .WithField("Previous Contents", StringEx.DefaultIfNullOrEmpty(e.MessageBefore?.Content, "Failed to retrieve previous message contents."))
      //   .WithField("New Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve new message contents."))
      //   .WithAuthorAndAttachmentInfo(e.Message!)
      //   .Send();
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
