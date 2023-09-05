using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using NellieBot.Extensions;

namespace NellieBot.Events
{
    public class GuildEvents
    {
        public static async Task GuildAvailable(DiscordClient _, GuildCreateEventArgs e)
        {
            if (Program.GuildSettings.GuildId == e.Guild.Id)
            {

            }
        }
    }
}
