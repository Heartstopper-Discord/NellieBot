using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus;
using NellieBot.Database;
using NellieBot.Extensions;

namespace NellieBot.Commands.Moderation
{
    public class ModerationCommands : ApplicationCommandModule
    {
        public required GuildSettings GuildSettings { private get; set; }

        [SlashCommand("ping", "A slash command made to test NellieBot!")]
        public async Task PingCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            /*using (var db = new DatabaseContext())
            {
                string resp = $"{GuildSettings.ActionLogChannel}";
                
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(resp));

            }*/
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("hi"));
        }
    }
}
