using DSharpPlus.Entities;
using DSharpPlus.Commands;
using NellieBot.Helper;
using DSharpPlus.Commands.ContextChecks;
using System.ComponentModel;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace NellieBot.Commands
{
  [HasRole(ModType.Mod)]
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

    [Command("privacy")]
    public async ValueTask PrivacyCommand(SlashCommandContext ctx)
    {
      await ctx.RespondAsync("The privacy policy for NellieBot can be found at https://github.com/Heartstopper-Discord/NellieBot/blob/main/privacy-policy.md");
    }

    // class Scores {
    //   public DiscordMessage message { get; set; }
    //   public int Yes { get; set; }
    //   public int Maybe { get; set; }
    //   public int ProbablyNot { get; set; }
    //   public int definitelyNot { get; set; }
    // }

    // [Command("count")]
    // public async ValueTask CountCommand(SlashCommandContext ctx)
    // {
    //   var channel = await ctx.Guild!.GetChannelAsync(1142536957126246450);
    //   await ctx.DeferResponseAsync();
      
    //   List<Scores> scores = new List<Scores>();
    //   ulong lastMessage = 1299720949062701056;
    //   while (true) {
    //     var msgs = channel.GetMessagesAfterAsync(lastMessage, 100);
    //     int count = 0;
    //     DiscordMessage currMsg = null;
    //     await foreach (var msg in msgs) {
    //       currMsg = msg; 
    //       count += 1;
    //       var reactions = msg.Reactions;
    //       int yesCount = reactions.FirstOrDefault(x => x.Emoji.Name == "🟢")?.Count ?? 0;
    //       int maybeCount = reactions.FirstOrDefault(x => x.Emoji.Name == "🟡")?.Count ?? 0;
    //       int probablyNotCount = reactions.FirstOrDefault(x => x.Emoji.Name == "🟠")?.Count ?? 0;
    //       int definitelyNotCount = reactions.FirstOrDefault(x => x.Emoji.Name == "🔴")?.Count ?? 0;
    //       int postedQuestionCount = reactions.FirstOrDefault(x => x.Emoji.Name == "✅")?.Count ?? 0;
    //       if (yesCount + maybeCount + probablyNotCount + definitelyNotCount > 0 && postedQuestionCount == 0) {
    //         scores.Add(new Scores { message = msg, Yes = yesCount, Maybe = maybeCount, ProbablyNot = probablyNotCount, definitelyNot = definitelyNotCount });
    //       }
    //     }
    //     if (count < 100) {
    //       break;
    //     }
    //     lastMessage = currMsg!.Id;
    //   }

    //   var message = scores.OrderByDescending(x => x.Yes);
    //   string output = "";
    //   foreach (var msg in message) {
    //     if (output.Length > 1800) {
    //       await ctx.Channel.SendMessageAsync(output);
    //       output = "";
    //     }
    //     output += $"{msg.message.JumpLink} - {(msg.Yes > 0 ? $"🟢: **{msg.Yes.ToString().PadRight(2)} " : "")}{(msg.Maybe > 0 ? $"🟡: {msg.Maybe.ToString().PadRight(2)} " : "")}{(msg.ProbablyNot > 0 ? $"🟠: {msg.ProbablyNot.ToString().PadRight(2)} " : "")}{(msg.definitelyNot > 0 ? $"🔴: {msg.definitelyNot.ToString().PadRight(2)}" : "")} \n";
    //   }
    //   if (output.Length > 0) {
    //     await ctx.Channel.SendMessageAsync(output);
    //   }
    //   await ctx.EditResponseAsync("Working");
    // }

    [Command("eventquestion")]
    public async ValueTask EventQuestionCommand(SlashCommandContext ctx, [Description("The question to be asked")] string question,
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
    public async ValueTask EventPaqCommand(SlashCommandContext ctx)
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
