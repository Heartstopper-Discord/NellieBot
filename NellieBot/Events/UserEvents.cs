using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NellieBot.Extensions;

namespace NellieBot.Events
{
    public class UserEvents
    {
        public static async Task GuildMemberAdded(DiscordClient _, GuildMemberAddEventArgs e)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Test"
            }
            .AddField("Hello", "hi");

            await e.Member.SendMessageAsync(embedBuilder);
        }

        public static async Task MessageUpdated(DiscordClient _, MessageUpdateEventArgs e)
        {
            if (e.Author.IsCurrent) return;
            if (e.Message.WebhookMessage) return;

            var logChannel = e.Guild.GetChannel(Program.GuildSettings.MessageLogChannel);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Author = new() { Name = "Message Edited", IconUrl = e.Author.AvatarUrl },
                Description = $"Location: {e.Channel.Mention} ({e.Channel.Name})",
                Timestamp = DateTime.Now
            }.AddField(
                "Previous Contents",
                StringEx.DefaultIfNullOrEmpty(e.MessageBefore?.Content, "Failed to retrieve previous message contents.").TrimForEmbed())
            .AddField(
                "New Contents",
                StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve new message contents.").TrimForEmbed())
            .AddAuthorAndAttachmentInfo(e.Message!);

            await logChannel.SendMessageAsync(embedBuilder);
        }


        public static async Task MessageDeleted(DiscordClient _, MessageDeleteEventArgs e)
        {
            if (e.Message.Author.IsCurrent) return;
            if (e.Message.WebhookMessage) return;

            var logChannel = e.Guild.GetChannel(Program.GuildSettings.MessageLogChannel);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.HotPink,
                Author = new() { Name = "Message Deleted", IconUrl = e.Message.Author.AvatarUrl },
                Description = $"Location: {e.Channel.Mention} ({e.Channel.Name})",
                Timestamp = DateTime.Now
            }.AddField(
                "Message Contents",
                StringEx.DefaultIfNullOrEmpty(e.Message?.Content, "Failed to retrieve message contents.").TrimForEmbed())
            .AddAuthorAndAttachmentInfo(e.Message!);

            await logChannel.SendMessageAsync(embedBuilder);
        }
    }
}