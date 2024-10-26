using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.ContextChecks.ParameterChecks;
using DSharpPlus.Entities;

namespace NellieBot.Helper
{
  public enum ModType
  {
    Mod,
    SrMod,
    Admin
  }

  public class HasRoleAttribute(ModType modType) : ContextCheckAttribute
  {
    public ModType ModType { get; private set; } = modType;
  }

  public class HasRole : IContextCheck<HasRoleAttribute>
  {
    public ValueTask<string?> ExecuteCheckAsync(HasRoleAttribute attribute, CommandContext ctx)
    {
      DiscordRole role = attribute.ModType switch
      {
        ModType.Mod => Program.DiscordConfig.Moderator,
        ModType.SrMod => Program.DiscordConfig.SeniorModerator,
        ModType.Admin => Program.DiscordConfig.Admin,
        _ => throw new NotImplementedException()
      };
      if (ctx.Member is not null && ctx.Member.Roles.Any(x => x == role))
        return ValueTask.FromResult<string?>(null);
      return ValueTask.FromResult<string?>("You do not have the required role to run this command.");
    }
  }
}
