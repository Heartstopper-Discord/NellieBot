using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using NellieBot.Extensions;
using NellieBot.Helper;

namespace NellieBot.Events
{
    public class UserEvents
    {
        public static async Task GuildMemberAdded(DiscordClient _, GuildMemberAddedEventArgs e) 
        {
            await new LogBuilder(LogType.GuildMemberAdded).
            WithAuthor(e.Member).
            WithField("Welcome: ",String.Concat(e.Member.Mention," (",e.Member.Username,")").
            WithField("ID: ",e.Member.Id);
        }

        public static async Task GuildMemberAdded(DiscordClient _, GuildMemberAddedEventArgs e) 
        {
            await new LogBuilder(LogType.GuildMemberRemoved).
            WithAuthor(e.Member).
            WithField("Goodbye: ",String.Concat(e.Member.Mention," (",e.Member.Username,")").
            WithField("ID: ",e.Member.Id);;
        }
    }
}
