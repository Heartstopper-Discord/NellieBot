using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NellieBot.Database.Collections;
using NellieBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Events
{
  public class ModalEvents
  {
    public static async Task ModalSubmitted(DiscordClient c, ModalSubmitEventArgs e)
    {
      var split = e.Interaction.Data.CustomId.Split(':');
      switch (split[0]) {
        case "automod-add":
          await AutomodCollection.AddAutomodRule(e.Values["label"], [.. e.Values["words"].Split(',').Select(x => x.Trim())], [.. e.Values["regex"].Split('\n')], e.Values["alert"]);
          await c.GetSlashCommands().RefreshCommands();

          await e.Interaction.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Rule added").AsEphemeral()
          );
          break;

        case "automod-edit":
          await AutomodCollection.EditAutomodRule(int.Parse(split[1]), e.Values["label"], [.. e.Values["words"].Split(',').Select(x => x.Trim())], [.. e.Values["regex"].Split('\n')], e.Values["alert"]);
          await c.GetSlashCommands().RefreshCommands();

          await e.Interaction.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Rule edited").AsEphemeral()
          );
          break;
      }
    }
  }
}
