using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace NellieBot.Helper
{
  public enum ModType
  {
    Mod,
    SrMod,
    Admin
  }
  public class HasRole : SlashCheckBaseAttribute
  {
    public ModType ModType;

    public HasRole(ModType modType)
    {
      ModType = modType;
    }

    public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
      DiscordRole role = ModType switch
      {
        ModType.Mod => Program.DiscordConfig.Moderator,
        ModType.SrMod => Program.DiscordConfig.SeniorModerator,
        ModType.Admin => Program.DiscordConfig.Admin,
        _ => throw new NotImplementedException()
      };

      if (ctx.Member.Roles.Any(x => x == role))
        return Task.FromResult(true);
      return Task.FromResult(false);
    }
  }
}
