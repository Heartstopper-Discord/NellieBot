using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NellieBot.Database.Collections;
using NellieBot.Database.Entities;
using NellieBot.Helper;

namespace NellieBot.Events
{
  public class ModalEvents
  {
    public static async Task ModalSubmitted(DiscordClient c, ModalSubmittedEventArgs e)
    {
      var split = e.Interaction.Data.CustomId.Split(':');
      if (split[0] == "automod-add") {
        if (string.IsNullOrEmpty(e.Values["words"]) && string.IsNullOrEmpty(e.Values["regex"])) {
          await e.Interaction.CreateResponseAsync(
            DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("No words or regexes supplied.").AsEphemeral()
          );
          return;
        }

        List<string> words = [.. e.Values["words"].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))];
        List<string> regexes = [.. e.Values["regex"].Split('\n').Where(x => !string.IsNullOrEmpty(x))];
        await AutomodCollection.AddAutomodRule(e.Values["label"], words, regexes, e.Values["alert"]);
        await Program.CommandsExtension.RefreshAsync();

        await e.Interaction.CreateResponseAsync(
          DiscordInteractionResponseType.ChannelMessageWithSource,
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
            DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("No words or regexes supplied.").AsEphemeral()
          );
          return;
        }
        AutomodData rule = await AutomodCollection.GetAutomodRule(int.Parse(split[1]));

        List<string> words = [.. e.Values["words"].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))];
        List<string> regexes = [.. e.Values["regex"].Split('\n').Where(x => !string.IsNullOrEmpty(x))];
        await AutomodCollection.EditAutomodRule(int.Parse(split[1]), e.Values["label"], words, regexes, e.Values["alert"]);
        await Program.CommandsExtension.RefreshAsync();

        await e.Interaction.CreateResponseAsync(
          DiscordInteractionResponseType.ChannelMessageWithSource,
          new DiscordInteractionResponseBuilder().WithContent("Rule edited").AsEphemeral()
        );
        LogBuilder log = new LogBuilder(LogType.AutomodRuleModified)
          .WithEventEmbed(e.Interaction.Channel, (DiscordMember)e.Interaction.User)
          .WithField("Label", e.Values["label"]);

        if (rule.Label != e.Values["label"]) {
          log.WithField("New Label", e.Values["label"]);
        }
        if (!rule.Words.SequenceEqual(words)) {
           log.WithField("Old Words", string.Join(',', rule.Words.Select(x => $"`{x}`")))
            .WithField("New Words", string.Join(',', words.Select(x => $"`{x}`")));
        }
        if (!rule.Regexes.SequenceEqual(regexes)) {
           log.WithField("Old Regexes", string.Join(',', rule.Regexes.Select(x => $"`{x}`")))
            .WithField("New Regexes", string.Join(',', regexes.Select(x => $"`{x}`")));
        }
        if (rule.Alert != e.Values["alert"]) {
          log.WithField("Old Alert", rule.Alert)
            .WithField("New Alert", e.Values["alert"]);
        }
        await log.Send();
        return;
      }

      else if (split[0] == "event-answer") {
        string question = e.Values["question"];
        string answer = string.Join('\n', e.Values["answer"].Split('\n').Select(x => "> " + x));
        string source = e.Values["source"];
        var res = $"## {question}\n{answer}\n\n*__Answer source:__ <{source}>*\n## <:Leaf1:976072863855558706><:Leaf2:1064274309423579137><:Leaf3:1064274307951382721><:Leaf4:1064274305493516298>";
        await e.Interaction.Channel.SendMessageAsync(res);

        await new LogBuilder(LogType.Say)
          .WithCodeField("Event message sent", res)
          .WithField("Channel", e.Interaction.Channel.Mention)
          .WithAuthorFooter((DiscordMember)e.Interaction.User)
          .Send();
        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sent!").AsEphemeral());

        return;
      }
    }
  }
}
