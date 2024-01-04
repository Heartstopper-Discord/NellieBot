using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Extensions
{
  static class DiscordEmbedExtensions
  {
    public static DiscordEmbedBuilder AddAuthorInfo(this DiscordEmbedBuilder b, DiscordUser u)
    {
      return b.AddField("Message Author:", $"{u.Mention} ({u.Username})")
        .AddField("Author ID:", u.Id.ToString());
    }

    public static DiscordEmbedBuilder AddAttachmentInfo(this DiscordEmbedBuilder b, DiscordMessage m)
    {
      if (m.Attachments.Any())
        b.AddField("Attachments:", $"{string.Join("\n", m.Attachments.Select(x => x.Url))}");
      return b;
    }

    public static DiscordEmbedBuilder AddAuthorAndAttachmentInfo(this DiscordEmbedBuilder b, DiscordMessage m)
    {
      return b.AddAuthorInfo(m.Author).AddAttachmentInfo(m);
    }
  }
}
