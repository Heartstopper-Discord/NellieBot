global using DSharpPlus;
using Microsoft.Extensions.Logging;
using NellieBot.Events;
using NellieBot.Database;
using Newtonsoft.Json;
using NellieBot.Commands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using NellieBot.Helper;

namespace NellieBot
{
  class Program
  {
    public static Config BotConfig;
    public static DiscordConfig DiscordConfig;

    static async Task Main(string[] args)
    {
      try
      {
        BotConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))!;
      }
      catch (Exception)
      {
        File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
        Console.WriteLine("No config exists. Please fill in config.json and restart.");
        return;
      }

      var clientBuilder = DiscordClientBuilder.CreateDefault(BotConfig.Token, DiscordIntents.All);

      clientBuilder.ConfigureEventHandlers(h => {
        h.HandleThreadCreated(GuildEvents.ThreadCreated);
        h.HandleModalSubmitted(ModalEvents.ModalSubmitted);
        h.HandleMessageCreated(GuildEvents.MessageCreated);
        h.HandleMessageUpdated(GuildEvents.MessageUpdated);
        h.HandleMessageDeleted(GuildEvents.MessageDeleted);
        h.HandleGuildMemberAdded(GuildEvents.GuildMemberAdded);
        h.HandleGuildMemberRemoved(GuildEvents.GuildMemberRemoved);
      });

      clientBuilder.UseInteractivity(new InteractivityConfiguration()
      {
        Timeout = TimeSpan.FromSeconds(30)
      });

      clientBuilder.SetLogLevel(LogLevel.Information);

      clientBuilder.UseCommands((IServiceProvider sp, CommandsExtension e) =>
      {
        e.AddCommands([typeof(UtilityCommands), typeof(AutomodCommands)]); // , typeof(WarnCommands)
        e.AddCheck<HasRole>();
        e.AddProcessor(new SlashCommandProcessor());
        e.CommandErrored += async (s, e) => {
          await ((SlashCommandContext)e.Context).RespondAsync("You do not have permission to use that command!", true);
          await new LogBuilder(LogType.Error)
            .WithField("Error", e.Exception.ToString())
            .WithField("Command", e.Context.Command.Name)
            .WithField("User", ((SlashCommandContext)e.Context).Member?.Mention ?? "Unknown")
            .Send();
        };
      }, new CommandsConfiguration()
      {
        UseDefaultCommandErrorHandler = false
      });

      var client = clientBuilder.Build();

      await client.ConnectAsync();
      DiscordConfig = new DiscordConfig(await client.GetGuildAsync(BotConfig.GuildId), BotConfig);

      await Task.Delay(-1);
    }
  }
}
