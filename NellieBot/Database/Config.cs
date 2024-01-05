using DSharpPlus.Entities;
using NellieBot.Database.Collections;

namespace NellieBot.Database
{
  public class Config
  {
    public string Token { get; set; } = "bot_token_here";
    public string DBHost { get; set; } = "localhost";
    public string DBUsername { get; set; } = "db_username_here";
    public string DBPassword { get; set; } = "db_password_here";
    public ulong GuildId { get; set; }
    public ulong ModeratorId { get; set; }
    public ulong SeniorModeratorId { get; set; }
    public ulong AdminId { get; set; }
    public ulong MessageLogChannel { get; set; }
    public ulong MemberLogChannel { get; set; }
    public ulong ActionLogChannel { get; set; }
    public ulong UtilityLogChannel { get; set; }
  }

  public class DiscordConfig
  {
    public DiscordGuild Guild { get; set; }
    public DiscordRole Moderator { get; set; }
    public DiscordRole SeniorModerator { get; set; }
    public DiscordRole Admin { get; set; }
    public DiscordChannel MessageLogChannel { get; set; }
    public DiscordChannel MemberLogChannel { get; set; }
    public DiscordChannel ActionLogChannel { get; set; }
    public DiscordChannel UtilityLogChannel { get; set; }
    public Dictionary<string, string> AutomodRules { get; set; } = [];

    public DiscordConfig(DiscordGuild g, Config c)
    {
      Guild = g;
      Moderator = g.GetRole(c.ModeratorId);
      SeniorModerator = g.GetRole(c.SeniorModeratorId);
      Admin = g.GetRole(c.AdminId);
      MessageLogChannel = g.GetChannel(c.MessageLogChannel);
      MemberLogChannel = g.GetChannel(c.MemberLogChannel);
      ActionLogChannel = g.GetChannel(c.ActionLogChannel);
      UtilityLogChannel = g.GetChannel(c.UtilityLogChannel);
      RefreshAutomodRules().Wait();
    }

    public async Task RefreshAutomodRules() {
      AutomodRules = (await AutomodCollection.GetAllAutomodRules()).ToDictionary(k => string.Join("|", k.Words.Concat(k.Regexes).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => $"({x})")), v => v.Alert);
    }
  }
}
