using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NellieBot.Extensions;
using NellieBot.Helper;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NellieBot.Events
{
  public class UserEvents
  {
    protected static async void sendAutoModLog(String reason,DiscordUser user, DiscordMessage msg, bool dmResult, Dictionary rules) {
      await new LogBuilder(LogType.AutoModRule)
        .WithActionEmbed(reason, "", user, ctx.User, dmResult)
        .WithField("Message ", msg.JumpLink)
        .WithFields("Rules caught ", rules)
        .Send();
    }

    protected static async Task AutoModHandler(MessageCreateEventArgs e) {

      bool success = false;
      Dictionary matchDict = new Dictionary<string,string>();

      DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
      {
        Title = "Ableist Terms Detected"
      };
      foreach (var entry in Program.DiscordConfig.AutomodRules)
      {
        var matches = Regex.Matches(e.Message.Content, entry.Key, RegexOptions.IgnoreCase);
        if (matches.Count != 0) {
          success = true;
          embedBuilder.AddField(entry.Value, string.Join(", ", matches.Select(x => x.Value)));
          matchDict.Add(entry.Key, string.Join(", ", matches.Select(x => x.Value)));
        }
      }
      try {
        if (success) {
          await ((DiscordMember)e.Author).SendMessageAsync(embedBuilder);
           sendAutoModLog("Warning item(s) caught in message",e.user,e.message,true,matchDict);
        }
      } catch (UnauthorizedException e) {
        sendAutoModLog("Warning item(s) caught in message. User has blocked Nellie.",e.user,e.message,false,matchDict);
      } catch (Exception e) {
        sendAutoModLog("Warning item(s) caught in message. General DM Error.",e.user,e.message,false,matchDict);
      }

    }
    public static async Task MessageCreated(DiscordClient _, MessageCreateEventArgs e)
    {
      if (e.Author.IsBot || e.Channel.IsPrivate) return;

      AutoModHandler(e);

    }

    public static async Task MessageUpdated(DiscordClient _, MessageUpdateEventArgs e)
    {
      if (e.Author.IsCurrent || e.Message.WebhookMessage) return;

      AutoModHandler(e);

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
