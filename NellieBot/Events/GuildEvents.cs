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

    public static async Task GuildMemberAdded(DiscordClient _, GuildMemberAddedEventArgs e)
    {
      await new LogBuilder(LogType.MemberJoined)
        .WithField("Welcome:", $"{e.Member.Mention} ({e.Member.Username})")
        .WithField("ID", e.Member.Id.ToString())
        .WithFooter($"Member count: {e.Guild.MemberCount}")
        .Send();
    }

    public static async Task GuildMemberRemoved(DiscordClient _, GuildMemberRemovedEventArgs e)
    {
      await new LogBuilder(LogType.MemberLeft)
        .WithField("Goodbye:", e.Member.Username)
        .WithField("ID", e.Member.Id.ToString())
        .WithFooter($"Member count: {e.Guild.MemberCount}")
        .Send();
    }

    protected static async Task AutomodHandler(DiscordMessage message, DiscordChannel c, DiscordUser? u) {
      bool detected = false;

      LogBuilder log = new LogBuilder(LogType.AutomodRuleBroken)
        .WithEventEmbed(c, u)
        .WithField("Link to message", message.JumpLink.ToString());

      DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

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
        if (u is not null) {
          await u.SendMessageAsync(embedBuilder);
          log.WithField("DM Success", "Yes");
        } else {
          log.WithField("DM Success", "No");
        }
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
      await AutomodHandler(e.Message, e.Channel, e.Author);
    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdatedEventArgs e)
    {
      if (e.Author?.IsCurrent == true || e.Message.WebhookMessage == true || e.Author?.IsBot == true) return;
      if (e.MessageBefore?.Content == e.Message?.Content) return;
      if (e.Channel.Id == 1126280165375352842) return;
      if (e.Message is not null) {
        await AutomodHandler(e.Message, e.Channel, e.Author);
      }

      await new LogBuilder(LogType.MessageUpdated)
        .WithEventEmbed(e.Channel, e.Message?.Author)
        .WithField("Previous Contents", StringEx.DefaultIfNullOrEmpty(e.MessageBefore?.Content, "Failed to retrieve previous message contents."))
        .WithField("New Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve new message contents."))
        .WithField("Link to message", StringEx.DefaultIfNullOrEmpty(e.Message?.JumpLink.ToString(), "Failed to retrieve message link."))
        .WithAuthorAndAttachmentInfo(e.Message)
        .Send();
    }

    public static async Task MessageDeleted(DiscordClient _, MessageDeletedEventArgs e)
    {
      if (e.Message.Author?.IsCurrent == true || e.Message.WebhookMessage == true) return;
      if (e.Channel.Id == 1126280165375352842) return;

      await new LogBuilder(LogType.MessageDeleted)
        .WithEventEmbed(e.Channel, e.Message.Author)
        .WithField("Message Contents", StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve message contents."))
        .WithAuthorAndAttachmentInfo(e.Message)
        .Send();
    }
  }
}
  