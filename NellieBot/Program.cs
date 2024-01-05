global using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Serilog;
using NellieBot.Events;
using NellieBot.Database;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NellieBot.Commands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using NellieBot.Database.Collections;

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

      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

      var logFactory = new LoggerFactory().AddSerilog();
      var discord = new DiscordClient(new DiscordConfiguration()
      {
        Token = BotConfig.Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.All,
        MinimumLogLevel = LogLevel.Information,
        LoggerFactory = logFactory
      });

      discord.UseInteractivity(new InteractivityConfiguration()
      {
        Timeout = TimeSpan.FromSeconds(30)
      });

      discord.MessageCreated += UserEvents.MessageCreated;
      // discord.MessageUpdated += UserEvents.MessageUpdated;
      // discord.MessageDeleted += UserEvents.MessageDeleted;
      discord.ModalSubmitted += ModalEvents.ModalSubmitted;

      await discord.ConnectAsync();

      DiscordConfig = new DiscordConfig(await discord.GetGuildAsync(BotConfig.GuildId), BotConfig);

      var slash = discord.UseSlashCommands(new SlashCommandsConfiguration
      {
        Services = new ServiceCollection().AddSingleton(DiscordConfig).BuildServiceProvider()
      });

      slash.SlashCommandErrored += ClientEvents.SlashCommandErrored;
      // slash.RegisterCommands<WarnCommands>();
      // slash.RegisterCommands<UtilityCommands>();
      slash.RegisterCommands<AutomodCommands>();

      await slash.RefreshCommands();

      await Task.Delay(-1);
    }
  }
}
