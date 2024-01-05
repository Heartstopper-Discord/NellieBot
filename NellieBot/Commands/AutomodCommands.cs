using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using NellieBot.Database;
using NellieBot.Database.Collections;
using NellieBot.Helper;

namespace NellieBot.Commands
{
  [SlashCommandPermissions(Permissions.ManageMessages)]
  [HasRole(ModType.SrMod)]
  [SlashCommandGroup("automod", "The parent command for all automod commands.")]
  public class AutomodCommands : ApplicationCommandModule
  {
    public class RuleChoiceProvider : IChoiceProvider
    {
      public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
      {
        var rules = await AutomodCollection.GetAllAutomodRules();
        return rules.Select(x => new DiscordApplicationCommandOptionChoice(x.Label, x.Id.ToString()));
      }
    }

    public required DiscordConfig Config { private get; set; }

    [SlashCommand("add", "Adds an automod rule category")]
    public async Task AddAutomodRule(InteractionContext ctx)
    {

      var modal = new DiscordInteractionResponseBuilder()
        .WithTitle("Add Automod Rule")
        .WithCustomId("automod-add")
        .AddComponents(
          new TextInputComponent(
            label: "Label",
            customId: "label",
            required: true,
            style: TextInputStyle.Short
        ))
        .AddComponents(
          new TextInputComponent(
            label: "Choose your words",
            customId: "words",
            required: true,
            style: TextInputStyle.Paragraph,
            placeholder: "Separate words or phrases with a comma (dog, cat, tiger)."
        ))
        .AddComponents(
          new TextInputComponent(
            label: "Use a regex pattern for advanced matching",
            customId: "regex",
            required: false,
            style: TextInputStyle.Paragraph,
            placeholder: "Use a new line for each regex pattern."
        ))
        .AddComponents(
          new TextInputComponent(
            label: "Alert to be DMed to the member",
            customId: "alert",
            required: true,
            style: TextInputStyle.Paragraph
        ));

      await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    [SlashCommand("edit", "Edits an automod rule category")]
    public async Task EditAutomodRule(InteractionContext ctx,     
      [ChoiceProvider(typeof(RuleChoiceProvider))]
      [Option("rule", "Rule to edit")] string ruleId)
    {
      var rule = await AutomodCollection.GetAutomodRule(int.Parse(ruleId));

      var modal = new DiscordInteractionResponseBuilder()
        .WithTitle("Edit Automod Rule")
        .WithCustomId($"automod-edit:{rule.Id}")
        .AddComponents(
          new TextInputComponent(
            label: "Label",
            customId: "label",
            required: true,
            style: TextInputStyle.Short,
            value: rule.Label
        ))
        .AddComponents(
          new TextInputComponent(
            label: "Choose your words",
            customId: "words",
            required: true,
            style: TextInputStyle.Paragraph,
            placeholder: "Separate words or phrases with a comma (dog, cat, tiger).",
            value: string.Join(", ", rule.Words)
        ))

        .AddComponents(
          new TextInputComponent(
            label: "Use a regex pattern for advanced matching",
            customId: "regex",
            required: false,
            style: TextInputStyle.Paragraph,
            placeholder: "Use a new line for each regex pattern.",
            value: string.Join('\n', rule.Regexes)
        ))
        .AddComponents(
          new TextInputComponent(
            label: "Alert to be DMed to the member",
            customId: "alert",
            required: true,
            style: TextInputStyle.Paragraph,
            value: rule.Alert
        ));

      await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    [SlashCommand("remove", "Removes an automod rule category")]
    public async Task RemoveAutomodRule(InteractionContext ctx,     
      [ChoiceProvider(typeof(RuleChoiceProvider))]
      [Option("rule", "Rule to remove")] string ruleId)
    {
      await AutomodCollection.RemoveAutomodRule(int.Parse(ruleId));
      await ctx.Client.GetSlashCommands().RefreshCommands();

      await ctx.CreateResponseAsync(
        InteractionResponseType.ChannelMessageWithSource,
        new DiscordInteractionResponseBuilder().WithContent("Rule removed").AsEphemeral()
      );    
    }
  }
}
