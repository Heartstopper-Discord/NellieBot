﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using NellieBot.Database;

namespace NellieBot.Commands
{
    public class UtilityCommands : ApplicationCommandModule
    {
        public required GuildSettings GuildSettings { private get; set; }

        [SlashCommand("speak", "Say something through Nellie.")]
        public async Task SpeakCommand(InteractionContext ctx, [Option("text", "The text that Nellie will say.")]string text)
        {
            await ctx.Channel.SendMessageAsync(text);
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Message sent.").AsEphemeral()
            );
        }
    }
}