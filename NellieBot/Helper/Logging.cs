using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using NellieBot.Extensions;

namespace NellieBot.Helper
{
  public enum LogType
  {
    Warn,
    RemoveWarn,
    Timeout,
    RemoveTimeout,
    MessageUpdated,
    MessageDeleted
  }

  public class LogBuilder
  {
    private DiscordEmbedBuilder Embed { get; set; }
    private DiscordChannel LogChannel { get; set; }

    public LogBuilder(LogType type)
    {
      Embed = new DiscordEmbedBuilder().WithTimestamp(DateTime.Now);
      switch (type)
      {
        case LogType.Warn:
          Embed = Embed.WithColor(DiscordColor.Red)
            .WithTitle("Warning");
          LogChannel = Program.DiscordConfig.ActionLogChannel;
          break;

        case LogType.RemoveWarn:
          Embed = Embed.WithColor(DiscordColor.Green)
            .WithTitle("Warning Removal");
          LogChannel = Program.DiscordConfig.ActionLogChannel;
          break;

        case LogType.Timeout:
          Embed = Embed.WithColor(DiscordColor.Orange)
            .WithTitle("Timeout");
          LogChannel = Program.DiscordConfig.ActionLogChannel;
          break;

        case LogType.RemoveTimeout:
          Embed = Embed.WithColor(DiscordColor.Orange)
            .WithTitle("Timeout Removed");
          LogChannel = Program.DiscordConfig.ActionLogChannel;
          break;

        case LogType.MessageUpdated:
          Embed = Embed.WithColor(DiscordColor.Yellow).WithAuthor("Message Edited");
          LogChannel = Program.DiscordConfig.MessageLogChannel;
          break;

        case LogType.MessageDeleted:
          Embed = Embed.WithColor(DiscordColor.HotPink).WithAuthor("Message Deleted");
          LogChannel = Program.DiscordConfig.MessageLogChannel;
          break;
      }
    }

    public LogBuilder WithActionEmbed(string? reason, string? note, DiscordUser u, DiscordUser by, string dmResult)
    {
      Embed = Embed
        .AddField("User:", $"{u.Mention} ({u.Username})\n{u.Id}")
        .AddField("Reason:", StringEx.DefaultIfNullOrEmpty(reason, "No reason provided."))
        .AddField("Note:", StringEx.DefaultIfNullOrEmpty(note, "No note provided."))
        .AddField("User Notification:", dmResult)
        .WithFooter($"Requested by {by.Username}", by.AvatarUrl);
      return this;
    }

    public LogBuilder WithEventEmbed(DiscordChannel c, DiscordMember u)
    {
      Embed = Embed
        .WithAuthor(Embed.Author.Name, iconUrl: u.AvatarUrl)
        .WithDescription($"Location: {c.Mention} ({c.Name})");
      return this;
    }

    public LogBuilder WithField(string name, string value, bool inline = false)
    {
      Embed.AddField(name.TrimForEmbed(), value.TrimForEmbed(), inline);
      return this;
    }

    public LogBuilder WithAuthorAndAttachmentInfo(DiscordMessage m)
    {
      Embed.AddAuthorAndAttachmentInfo(m);
      return this;
    }

    public async Task Send()
    {
      await LogChannel.SendMessageAsync(Embed);
    }
  }

  public static class Logging
  {
    public static DiscordEmbedBuilder Test(this DiscordEmbedBuilder a)
    {
      return a;
    }

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
