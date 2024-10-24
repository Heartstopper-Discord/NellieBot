using DSharpPlus.Entities;
using DSharpPlus.Commands;
using NellieBot.Helper;
using DSharpPlus.Commands.ContextChecks;
using System.ComponentModel;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace NellieBot.Commands
{
  // [HasRole(ModType.Mod)]
  [RequirePermissions(DiscordPermissions.ManageMessages)]
  public class UtilityCommands
  {
    [Command("say")]
    public async ValueTask SayCommand(SlashCommandContext ctx, [Description("The text that Nellie will say.")] string text)
    {
      await ctx.Channel.SendMessageAsync(text);
      await ctx.RespondAsync("Message sent.", true);
      await new LogBuilder(LogType.Say)
        .WithCodeField("Message sent", text)
        .WithField("Channel", ctx.Channel.Mention)
        .WithAuthorFooter(ctx.Member)
        .Send();
    }

    [Command("eventquestion")]
    public async ValueTask EventSayCommand(SlashCommandContext ctx, [Description("The question to be asked")] string question,
                                           [Description("The user who asked the question originally")] DiscordUser user)
    {
      string message = $"## {question}\n*__Asked by__: {user.Mention}*\n## <:Leaf1:976072863855558706><:Leaf2:1064274309423579137><:Leaf3:1064274307951382721><:Leaf4:1064274305493516298>";
      var msg = await ctx.Channel.SendMessageAsync(message);
      await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":arrow_up:"));
      await new LogBuilder(LogType.Say)
        .WithCodeField("Event message sent", message)
        .WithAuthorFooter(ctx.Member)
        .Send();
      await ctx.RespondAsync("Message sent.", true);
    }

    [Command("eventpaq")]
    public async ValueTask EventAnswerCommand(SlashCommandContext ctx)
    {
      var modal = new DiscordInteractionResponseBuilder()
        .WithTitle("Add Event Answer")
        .WithCustomId("event-answer")
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Question",
            customId: "question",
            required: true,
            style: DiscordTextInputStyle.Short
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Answer",
            customId: "answer",
            required: true,
            style: DiscordTextInputStyle.Paragraph,
            placeholder: "No need to indent with >.",
            max_length: 1800
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Answer Source",
            customId: "source",
            required: true,
            style: DiscordTextInputStyle.Short
        ));
      await ctx.RespondWithModalAsync(modal);
    }
  }
}
