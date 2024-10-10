using DSharpPlus.Entities;
using DSharpPlus.Commands;
using NellieBot.Database;
using NellieBot.Helper;
using DSharpPlus.Commands.ContextChecks;
using System.ComponentModel;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace NellieBot.Commands
{
  [RequirePermissions(DiscordPermissions.ManageMessages)]
  public class UtilityCommands
  {
    [Command("speak")]
    [HasRole(ModType.Mod)]
    public async ValueTask SpeakCommand(SlashCommandContext ctx, [Description("The text that Nellie will say.")] string text)
    {
      await ctx.Channel.SendMessageAsync(text);
      await ctx.Channel.SendMessageAsync(Program.DiscordConfig.ActionLogChannel.Name);

      await ctx.RespondAsync("Message sent.", true);
    }
  }
}
