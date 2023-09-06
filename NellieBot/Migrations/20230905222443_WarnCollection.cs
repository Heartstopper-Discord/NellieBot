using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NellieBot.Migrations
{
    /// <inheritdoc />
    public partial class WarnCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelSettings");

            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.CreateTable(
                name: "WarnData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarnData", x => new { x.Id, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarnData");

            migrationBuilder.CreateTable(
                name: "ChannelSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActionLogChannel = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberLogChannel = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageLogChannel = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UtilityLogChannel = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.GuildId);
                });
        }
    }
}
