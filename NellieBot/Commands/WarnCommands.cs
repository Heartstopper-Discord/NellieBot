using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using NellieBot.Database;
using NellieBot.Extensions;
using NellieBot.Database.Collections;
using NellieBot.Helper;
using DSharpPlus.Interactivity.Extensions;
using System.Diagnostics;

namespace NellieBot.Commands
{
  [SlashCommandPermissions(Permissions.ManageMessages)]
  [HasRole(ModType.Mod)]
  [SlashCommandGroup("warn", "The parent command for all warn commands.")]
  public class WarnCommands : ApplicationCommandModule
  {
    public required Config GuildSettings { private get; set; }

    [SlashCommand("user", "Warns a user.")]
    public async Task WarnUserCommand(InteractionContext ctx,
      [Option("user", "Person to warn")] DiscordUser user,
      [Option("reason", "Reason for warning (sent to user)")] string reason = "",
      [Option("note", "Note for moderators that goes on record")] string note = "",
      [Option("dm", "Whether to send a DM (defaults to true)")] bool dm = true)
    {
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
      var member = (DiscordMember)user;

      await WarnCollection.AddWarn(member, reason, note);
      await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Warned user."));

      var numWarns = WarnCollection.GetWarnCount(member);

      var dmResult = await member.SendModerationDM(dm,
        $"Warning {numWarns} in {ctx.Guild.Name}",
        $"**Reason:** {reason}");

      await new LogBuilder(LogType.Warn)
        .WithActionEmbed(reason, note, user, ctx.User, dmResult)
        .WithField("Total strikes", numWarns.ToString())
        .Send();
    }

    [SlashCommand("remove", "Removes a warning from a user")]
    public async Task WarnRemoveCommand(InteractionContext ctx,
      [Option("user", "Person to remove warning from")] DiscordUser user,
      [Option("dm", "Whether to send a DM (defaults to false)")] bool dm = false)
    {
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

      var member = (DiscordMember)user;
      var warnings = WarnCollection.GetWarns(member);

      if (!warnings.Any())
      {
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{member.DisplayName} has no warnings."));
        return;
      }

      var options = warnings.Select((x, i) => new DiscordSelectComponentOption($"Warning {i + 1}", x.Id.ToString(), $"Reason: {StringEx.DefaultIfNullOrEmpty(x.Reason, "No reason provided")}"));
      var dropdown = new DiscordSelectComponent("warn_remove", "Select warning to remove...", options, false, 1, 1);
      var msg = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddComponents(dropdown));
      var res = await msg.WaitForSelectAsync("warn_remove");
      if (res.TimedOut || !res.Result.Values.Any())
      {
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Timed out - no warning removed."));
        return;
      }
      var selected = res.Result.Values[0];

      WarnCollection.RemoveWarn(member, int.Parse(selected));
      await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Removed warning from user."));


      var numWarns = WarnCollection.GetWarnCount(member);

      var dmResult = await member.SendModerationDM(dm,
        $"Warning removal in {ctx.Guild.Name}",
        $"You have had a warning removed. You now have {numWarns} active warnings.");

      await new LogBuilder(LogType.RemoveWarn)
        .WithActionEmbed(null, null, user, ctx.User, dmResult)
        .WithField("Total strikes", numWarns.ToString())
        .Send();
    }

    [SlashCommand("list", "Lists warnings for a user")]
    public async Task WarnListCommand(InteractionContext ctx,
      [Option("user", "Person to list warnings for")] DiscordUser user)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      var member = (DiscordMember)user;
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      var warnings = WarnCollection.GetWarns(member);

      Console.WriteLine(stopwatch.ElapsedMilliseconds);
      if (!warnings.Any())
      {
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{member.DisplayName} has no warnings."));
        return;
      }

      var embed = new DiscordEmbedBuilder()
      {
        Color = DiscordColor.Red,
        Title = $"Warnings for {member.DisplayName} ({member.Username})"
      };
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      for (int i = 0; i < warnings.Count; i++)
      {
        var x = warnings[i];
        embed.AddField($"Warning {i + 1}",
          $"Created at: {Formatter.Timestamp(x.DateTime, TimestampFormat.LongDateTime)} " +
          $"({Formatter.Timestamp(x.DateTime, TimestampFormat.RelativeTime)})\n" +
          $"Reason: {StringEx.DefaultIfNullOrEmpty(x.Reason, "No reason provided")}\n" +
          $"Note: {StringEx.DefaultIfNullOrEmpty(x.Note, "No note provided")}");
      }
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
  }
}
