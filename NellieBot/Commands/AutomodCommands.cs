using DSharpPlus.Entities;
using DSharpPlus.Commands;
using NellieBot.Database;
using NellieBot.Database.Collections;
using NellieBot.Helper;
using System.ComponentModel;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;

namespace NellieBot.Commands
{
  [RequirePermissions(DiscordPermissions.ManageMessages)]
  [HasRole(ModType.SrMod)]
  [Command("automod")]
  [Description("The parent command for all automod commands.")]
  public class AutomodCommands
  {
    public class RuleAutocompleteProvider : IAutoCompleteProvider
    {
      public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext _)
      {
        var rules = await AutomodCollection.GetAllAutomodRules();
        return rules.Select(x => new DiscordAutoCompleteChoice(x.Label, x.Id.ToString()));
      }
    }

    public required DiscordConfig Config { private get; set; }

    [Command("add")]
    [Description("Adds an automod rule category")]
    public async Task AddAutomodRule(SlashCommandContext ctx)
    {
      var modal = new DiscordInteractionResponseBuilder()
        .WithTitle("Add Automod Rule")
        .WithCustomId("automod-add")
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Label",
            customId: "label",
            required: true,
            style: DiscordTextInputStyle.Short,
            max_length: 256
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Choose your words",
            customId: "words",
            required: false,
            style: DiscordTextInputStyle.Paragraph,
            placeholder: "Separate words or phrases with a comma (dog, cat, tiger)."
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Use a regex pattern for advanced matching",
            customId: "regex",
            required: false,
            style: DiscordTextInputStyle.Paragraph,
            placeholder: "Use a new line for each regex pattern."
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Alert to be DMed to the member",
            customId: "alert",
            required: true,
            style: DiscordTextInputStyle.Paragraph,
            max_length: 1024
        ));

      await ctx.RespondWithModalAsync(modal);
    }

    [Command("edit")]
    [Description("Edits an automod rule category")]
    public async Task EditAutomodRule(SlashCommandContext ctx,     
      [SlashAutoCompleteProvider<RuleAutocompleteProvider>]
      [Parameter("rule")][Description("Rule to edit")] string ruleId)
    {
      var rule = await AutomodCollection.GetAutomodRule(int.Parse(ruleId));
      if (rule == null) {
        await ctx.RespondAsync("Rule not found", true);
        return;
      }

      var modal = new DiscordInteractionResponseBuilder()
        .WithTitle("Edit Automod Rule")
        .WithCustomId($"automod-edit:{rule.Id}")
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Label",
            customId: "label",
            required: true,
            style: DiscordTextInputStyle.Short,
            value: rule.Label,
            max_length: 256
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Choose your words",
            customId: "words",
            required: false,
            style: DiscordTextInputStyle.Paragraph,
            placeholder: "Separate words or phrases with a comma (dog, cat, tiger).",
            value: string.Join(", ", rule.Words)
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Use a regex pattern for advanced matching",
            customId: "regex",
            required: false,
            style: DiscordTextInputStyle.Paragraph,
            placeholder: "Use a new line for each regex pattern.",
            value: string.Join('\n', rule.Regexes)
        ))
        .AddComponents(
          new DiscordTextInputComponent(
            label: "Alert to be DMed to the member",
            customId: "alert",
            required: true,
            style: DiscordTextInputStyle.Paragraph,
            value: rule.Alert,
            max_length: 1024
        ));

      await ctx.RespondWithModalAsync(modal);
    }

    [Command("remove")]
    [Description("Removes an automod rule category")]
    public async Task RemoveAutomodRule(SlashCommandContext ctx,     
      [SlashAutoCompleteProvider<RuleAutocompleteProvider>]
      [Parameter("rule")][Description("Rule to delete")] string ruleId)
    {
      var rule = await AutomodCollection.GetAutomodRule(int.Parse(ruleId));
      if (rule == null) {
        await ctx.RespondAsync("Rule not found", true);
        return;
      }
      bool success = await AutomodCollection.RemoveAutomodRule(int.Parse(ruleId));
      if (!success) {
        await ctx.RespondAsync("Failed to remove rule", true);
        return;
      }
      await ctx.RespondAsync("Rule removed", true);

      await new LogBuilder(LogType.AutomodRuleRemoved)
        .WithEventEmbed(ctx.Channel, ctx.Member!)
        .WithField("Label", rule.Label)
        .WithField("Words", string.Join(',', rule.Words.Select(x => $"`{x}`")))
        .WithField("Regex", string.Join(',', rule.Regexes.Select(x => $"`{x}`")))
        .WithField("Alert", rule.Alert)
        .Send();
    }
  }
}
