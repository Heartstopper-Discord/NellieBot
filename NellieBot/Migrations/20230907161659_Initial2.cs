using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NellieBot.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WarnData",
                table: "WarnData");

            migrationBuilder.RenameTable(
                name: "WarnData",
                newName: "Warns");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Warns",
                table: "Warns",
                columns: new[] { "Id", "UserId" });

            migrationBuilder.CreateTable(
                name: "Timeouts",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeouts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Timeouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Warns",
                table: "Warns");

            migrationBuilder.RenameTable(
                name: "Warns",
                newName: "WarnData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarnData",
                table: "WarnData",
                columns: new[] { "Id", "UserId" });
        }
    }
}
