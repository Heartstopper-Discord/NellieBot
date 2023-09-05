using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NellieBot.Migrations
{
    /// <inheritdoc />
    public partial class GuildSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildSettings",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "GuildSettings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GuildSettings",
                newName: "UtilityLogChannel");

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "UtilityLogChannel",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<ulong>(
                name: "ActionLogChannel",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "MemberLogChannel",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "MessageLogChannel",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildSettings",
                table: "GuildSettings",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildSettings",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "ActionLogChannel",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "MemberLogChannel",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "MessageLogChannel",
                table: "GuildSettings");

            migrationBuilder.RenameColumn(
                name: "UtilityLogChannel",
                table: "GuildSettings",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "GuildId",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "GuildSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "GuildSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildSettings",
                table: "GuildSettings",
                column: "Id");
        }
    }
}
