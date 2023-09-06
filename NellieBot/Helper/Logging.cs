using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using NellieBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Helper
{
    public class ModerationTypes
    {
        private string Title { set ; get; }
        private DiscordColor Color { set; get; }

        public static ModerationTypes Warn = new() { Title = "Warning", Color = DiscordColor.Red };
        public static ModerationTypes Timeout = new() { Title = "Timeout", Color = DiscordColor.Orange };

        public DiscordEmbedBuilder GetBaseEmbed(string? reason, DiscordUser u, DiscordUser by, string dmResult)
        {
            return new DiscordEmbedBuilder()
            {
                Title = Title,
                Timestamp = DateTime.Now,
                Color = Color,
            }
            .AddField("User:", $"{u.Mention} ({u.Username})\n{u.Id}")
            .AddField("Reason:", StringEx.DefaultIfNullOrEmpty(reason, "No reason provided."))
            .AddField("User Notification:", dmResult)
            .WithFooter($"Requested by {by.Username}", by.AvatarUrl);
        }
    }

    public static class Logging
    {

        public static async Task<string> SendModerationDM(this DiscordMember m, bool dm, string title, string desc)
        {
            if (!dm) return "Direct message notification disabled.";

            var response = "User notified with direct message.";

            var dmEmbed = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = desc
            };

            try
            {
                await m.SendMessageAsync(dmEmbed);
            }
            catch (UnauthorizedException)
            {
                response = "Failed to notify user with direct message.";
            }
            return response;
        }
    }
}
