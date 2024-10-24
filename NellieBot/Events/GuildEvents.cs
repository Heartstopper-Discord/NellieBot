using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using NellieBot.Extensions;
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

    public static async Task MessageCreated(DiscordClient _, MessageCreatedEventArgs e)
    {
      if (e.Author.IsBot || e.Channel.IsPrivate) return;
      await AutomodHandler(e.Message, e.Channel, (DiscordMember)e.Author);
    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdatedEventArgs e)
    {
      if (e.Author?.IsCurrent == true || e.Message.WebhookMessage == true || e.Author?.IsBot == true) return;
      await AutomodHandler(e.Message, e.Channel, (DiscordMember)e.Author!);

      await new LogBuilder(LogType.MessageUpdated)
        .WithEventEmbed(e.Channel, (DiscordMember)e.Message.Author!)
        .WithCodeField("Previous Contents", StringEx.DefaultIfNullOrEmpty(e.MessageBefore?.Content, "Failed to retrieve previous message contents."))
        .WithCodeField("New Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve new message contents."))
        .WithAuthorAndAttachmentInfo(e.Message!)
        .Send();
    }

    public static async Task MessageDeleted(DiscordClient _, MessageDeletedEventArgs e)
    {
      if (e.Message.Author?.IsCurrent == true || e.Message.WebhookMessage == true) return;

      await new LogBuilder(LogType.MessageDeleted)
        .WithEventEmbed(e.Channel, (DiscordMember)e.Message.Author!)
        .WithCodeField("Message Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve message contents."))
        .WithAuthorAndAttachmentInfo(e.Message!)
        .Send();
    }
  }
}
  