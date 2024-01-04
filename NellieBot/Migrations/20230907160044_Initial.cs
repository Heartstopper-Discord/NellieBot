using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NellieBot.Migrations
{
  /// <inheritdoc />
  public partial class Initial : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "WarnData",
        columns: table => new
        {
          Id = table.Column<int>(type: "integer", nullable: false),
          UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
          Reason = table.Column<string>(type: "text", nullable: false),
          Note = table.Column<string>(type: "text", nullable: false),
          DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
    }
  }
}
