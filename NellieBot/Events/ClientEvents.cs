using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using NellieBot.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Events
{
  public class ClientEvents
  {
    public static async Task SlashCommandErrored(SlashCommandsExtension _, SlashCommandErrorEventArgs e)
    {
      if (e.Exception is SlashExecutionChecksFailedException ex)
      {
        foreach (var check in ex.FailedChecks)
        {
          if (check is HasRole)
            await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
              new DiscordInteractionResponseBuilder().WithContent("You do not have permission to use this command!").AsEphemeral());
        }
      }
    }
  }
}
