using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using NellieBot.Database;
using NellieBot.Extensions;
using NellieBot.Database.Collections;
using NellieBot.Database.Entities;
using NellieBot.Helper;
using System.Diagnostics;

namespace NellieBot.Commands
{
  [SlashCommandPermissions(Permissions.ManageMessages)]
  [HasRole(ModType.Mod)]
  [SlashCommandGroup("timeout", "The parent command for all timeout commands.")]
  public class TimeoutCommands : ApplicationCommandModule
  {
    public required DiscordConfig Config { private get; set; }

    [SlashCommand("user", "Times out a user.")]
    public async Task TimeoutUserCommand(InteractionContext ctx,
      [Option("user", "Person to warn")] DiscordUser user,
      [Option("reason", "Reason for timing out (sent to user)")] string reason = "",
      [Option("length", "Length of timeout (TEMP *should we change to a option list like lily or do custom minute count?*")] int length = 0,
      [Option("dm", "Whether to send a DM (defaults to true)")] bool dm = true)
    {
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
      var member = (DiscordMember)user;

      int hours = length / 60;
      int minutes = length % 60;
      TimeSpan timespan = new TimeSpan(hours, minutes, 0);

      await TimeoutCollection.AddTimeout(member, reason, timespan);
      await member.TimeoutAsync(DateTimeOffset.Now + timespan, reason);
      await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Timed out user."));

      var numTimeouts = await TimeoutCollection.GetTimeoutCount(member);

      var dmResult = await member.SendModerationDM(dm,
        $"Timeout {numTimeouts} in {ctx.Guild.Name} for {length} minutes",
        $"**Reason:** {reason}");

      await new LogBuilder(LogType.Timeout)
        .WithActionEmbed(reason, "", user, ctx.User, dmResult)
        .WithField("Total strikes", numTimeouts.ToString())
        .WithField("Timeout length ", length + " minutes")
        .Send();
    }

    [SlashCommand("remove", "Removes a timeout from a user")]
    public async Task TimeoutRemoveCommand(InteractionContext ctx,
      [Option("user", "Person to remove timeout from")] DiscordUser user,
      [Option("reason", "Reason timeout was removed early")] string reason = "",
      [Option("dm", "Whether to send a DM (defaults to false)")] bool dm = false)
    {
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

      var member = (DiscordMember)user;
      TimeoutData timeout = await TimeoutCollection.GetCurrentTimeout(member);

      if (timeout == null)
      {
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{member.DisplayName} has no current timeout."));
        return;
      }

      ///TimeoutCollection.RemoveTimeout(member, timeout.Id); - Assuming we don't want to remove the log of timeouts but maybe I'm wrong?
      await member.TimeoutAsync(DateTime.Now);
      await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Removed timeout from user."));


      var numTimeouts = await TimeoutCollection.GetTimeoutCount(member);

      var dmResult = await member.SendModerationDM(dm,
        $"Timeout removal in {ctx.Guild.Name}",
        $"You have had your timeout removed.");

      await new LogBuilder(LogType.RemoveTimeout)
        .WithActionEmbed(reason, null, user, ctx.User, dmResult)
        .Send();
    }

    [SlashCommand("list", "Lists timeouts for a user")]
    public async Task TimeoutListCommand(InteractionContext ctx,
      [Option("user", "Person to list timeouts for")] DiscordUser user)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      var member = (DiscordMember)user;
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      var timeouts = await TimeoutCollection.GetTimeouts(member);

      Console.WriteLine(stopwatch.ElapsedMilliseconds);
      if (!timeouts.Any())
      {
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{member.DisplayName} has never been timed out."));
        return;
      }

      var embed = new DiscordEmbedBuilder()
      {
        Color = DiscordColor.Red,
        Title = $"Timeouts for {member.DisplayName} ({member.Username})"
      };
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      for (int i = 0; i < timeouts.Count; i++)
      {
        var x = timeouts[i];
        DateTime until = x.Until;
        TimeSpan timeLength = x.Until - x.DateTime;
        int minuteLength = timeLength.Hours * 60 + timeLength.Minutes;
        embed.AddField($"Timeout {i + 1}",
          $"Created at: {Formatter.Timestamp(x.DateTime, TimestampFormat.LongDateTime)} " +
          $"({Formatter.Timestamp(x.DateTime, TimestampFormat.RelativeTime)})\n" +
          $"Reason: {StringEx.DefaultIfNullOrEmpty(x.Reason, "No reason provided")}\n" +
          $"Lasting Timeout Length: {minuteLength} minutes");
      }
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

      await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
  }
}
