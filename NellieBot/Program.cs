﻿global using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using NellieBot.Commands.Moderation;
using Serilog;
using NellieBot.Events;
using NellieBot.Database;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace NellieBot
{
    class Program
    {
        public static GuildSettings GuildSettings;
        static async Task Main(string[] args)
        {
            try
            {
                GuildSettings = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText("config.json"))!; 
            } catch (Exception) 
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new GuildSettings() { Token = "bot_token_here" },Formatting.Indented));
                Console.WriteLine("No config exists. Please fill in config.json and restart.");
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var logFactory = new LoggerFactory().AddSerilog();
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = GuildSettings.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Information,
                LoggerFactory = logFactory
            });

            var slash = discord.UseSlashCommands(new SlashCommandsConfiguration() 
            { 
                Services = new ServiceCollection().AddSingleton(GuildSettings).BuildServiceProvider()
            });

            slash.RegisterCommands<ModerationCommands>();

            discord.GuildMemberAdded += UserEvents.GuildMemberAdded;
            discord.MessageUpdated += UserEvents.MessageUpdated;
            discord.MessageDeleted += UserEvents.MessageDeleted;

            discord.GuildAvailable += GuildEvents.GuildAvailable;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}