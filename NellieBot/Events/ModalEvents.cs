using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using NellieBot.Database.Collections;
using NellieBot.Database.Entities;
using NellieBot.Helper;

namespace NellieBot.Events
{
  public class ModalEvents
  {
    public static async Task ModalSubmitted(DiscordClient c, ModalSubmitEventArgs e)
    {
      var split = e.Interaction.Data.CustomId.Split(':');
      if (split[0] == "automod-add") {
        if (string.IsNullOrEmpty(e.Values["words"]) && string.IsNullOrEmpty(e.Values["regex"])) {
          await e.Interaction.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("No words or regexes supplied.").AsEphemeral()
          );
          return;
        }

        List<string> words = [.. e.Values["words"].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))];
        List<string> regexes = [.. e.Values["regex"].Split('\n').Where(x => !string.IsNullOrEmpty(x))];
        await AutomodCollection.AddAutomodRule(e.Values["label"], words, regexes, e.Values["alert"]);
        await c.GetSlashCommands().RefreshCommands();

        await e.Interaction.CreateResponseAsync(
          InteractionResponseType.ChannelMessageWithSource,
          new DiscordInteractionResponseBuilder().WithContent("Rule added").AsEphemeral()
        );
        await new LogBuilder(LogType.AutomodRuleAdded)
          .WithEventEmbed(e.Interaction.Channel, (DiscordMember)e.Interaction.User)
          .WithField("Label", e.Values["label"])
          .WithField("Words", string.Join(',', words.Select(x => $"`{x}`")))
          .WithField("Regex", string.Join(',', regexes.Select(x => $"`{x}`")))
          .WithField("Alert Message", e.Values["alert"])
          .Send();
        return;
      }
      else if (split[0] == "automod-edit") {
        if (string.IsNullOrEmpty(e.Values["words"]) && string.IsNullOrEmpty(e.Values["regex"])) {
          await e.Interaction.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("No words or regexes supplied.").AsEphemeral()
          );
          return;
        }
        AutomodData rule = await AutomodCollection.GetAutomodRule(int.Parse(split[1]));

        List<string> words = [.. e.Values["words"].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))];
        List<string> regexes = [.. e.Values["regex"].Split('\n').Where(x => !string.IsNullOrEmpty(x))];
        await AutomodCollection.EditAutomodRule(int.Parse(split[1]), e.Values["label"], words, regexes, e.Values["alert"]);
        await c.GetSlashCommands().RefreshCommands();

        await e.Interaction.CreateResponseAsync(
          InteractionResponseType.ChannelMessageWithSource,
          new DiscordInteractionResponseBuilder().WithContent("Rule edited").AsEphemeral()
        );
        LogBuilder log = new LogBuilder(LogType.AutomodRuleModified)
          .WithEventEmbed(e.Interaction.Channel, (DiscordMember)e.Interaction.User)
          .WithField("Label", e.Values["label"]);

        if (rule.Label != e.Values["label"]) {
          log = log.WithField("New Label", e.Values["label"]);
        }
        if (!rule.Words.SequenceEqual(words)) {
           log = log.WithField("Old Words", string.Join(',', rule.Words.Select(x => $"`{x}`")))
            .WithField("New Words", string.Join(',', words.Select(x => $"`{x}`")));
        }
        if (!rule.Regexes.SequenceEqual(regexes)) {
           log = log.WithField("Old Regexes", string.Join(',', rule.Regexes.Select(x => $"`{x}`")))
            .WithField("New Regexes", string.Join(',', regexes.Select(x => $"`{x}`")));
        }
        if (rule.Alert != e.Values["alert"]) {
          log = log.WithField("Old Alert", rule.Alert)
            .WithField("New Alert", e.Values["alert"]);
        }
        await log.Send();
        return;
      }
    }
  }
}
