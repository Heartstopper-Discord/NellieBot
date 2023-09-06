namespace NellieBot.Database
{
    public class GuildSettings
    {
        public required string Token { get; set; }
        public ulong GuildId { get; set; }
        public ulong ModeratorId { get; set; }
        public ulong SeniorModeratorId { get; set; }
        public ulong AdminId {  get; set; }
        public ulong MessageLogChannel { get; set; }
        public ulong MemberLogChannel { get; set; }
        public ulong ActionLogChannel { get; set; } 
        public ulong UtilityLogChannel { get; set; }
    }
}
