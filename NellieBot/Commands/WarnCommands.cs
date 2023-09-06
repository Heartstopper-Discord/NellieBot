using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus;
using NellieBot.Database;
using NellieBot.Extensions;
using NellieBot.Database.Collections;
using DSharpPlus.Exceptions;
using NellieBot.Helper;
using DSharpPlus.Interactivity.Extensions;

namespace NellieBot.Commands
{
    [SlashCommandGroup("warn", "The parent command for all warn commands.")]
    public class WarnCommands : ApplicationCommandModule
    {
        public required GuildSettings GuildSettings { private get; set; }

        [SlashCommand("user", "Warns a user.")]
        public async Task WarnUserCommand(InteractionContext ctx, 
            [Option("user", "Person to warn")]DiscordUser user,
            [Option("reason", "Reason for warning (sent to user)")] string reason = "",
            [Option("note", "Note for moderators that goes on record")] string note = "",
            [Option("dm", "Whether to send a DM (defaults to true)")] bool dm = true)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
            var member = (DiscordMember)user;

            WarnCollection.AddWarn(member, reason, note);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Warned user."));

            var numWarns = WarnCollection.GetWarnCount(member);

            var dmResult = await member.SendModerationDM(dm,
                $"Warning {numWarns} in {ctx.Guild.Name}", 
                $"**Reason:** {reason}");

            var embed = ModerationTypes.Warn
                .GetBaseEmbed(reason, user, ctx.User, dmResult)
                .AddField("Total strikes",numWarns.ToString());

            await ctx.Guild.GetChannel(GuildSettings.ActionLogChannel).SendMessageAsync(embed);
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

            var embed = ModerationTypes.Warn
                .GetBaseEmbed(null, user, ctx.User, dmResult)
                .AddField("Total strikes", numWarns.ToString());

            await ctx.Guild.GetChannel(GuildSettings.ActionLogChannel).SendMessageAsync(embed);
        }

        [SlashCommand("list", "Lists warnings for a user")]
        public async Task WarnListCommand(InteractionContext ctx,
            [Option("user", "Person to list warnings for")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var member = (DiscordMember)user;
            var warnings = WarnCollection.GetWarns(member);

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

            for (int i = 0; i < warnings.Count; i++)
            {
                var x = warnings[i];
                embed.AddField($"Warning {i + 1}", 
                    $"Created at {Formatter.Timestamp(x.DateTime, TimestampFormat.LongDateTime)} " +
                    $"{Formatter.Timestamp(x.DateTime, TimestampFormat.RelativeTime)}\n" +
                    $"Reason: {StringEx.DefaultIfNullOrEmpty(x.Reason, "No reason provided")}");
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
