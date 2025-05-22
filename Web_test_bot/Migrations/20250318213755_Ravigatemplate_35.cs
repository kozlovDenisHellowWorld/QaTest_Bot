using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyEnteties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntyName = table.Column<string>(type: "TEXT", nullable: false),
                    EntyCode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    ParentChatId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyEnteties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyEnteties_MyChats_ParentChatId",
                        column: x => x.ParentChatId,
                        principalTable: "MyChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyWorkDetales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntyName = table.Column<string>(type: "TEXT", nullable: false),
                    EntyCode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    Key = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    ParenId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyWorkDetales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyWorkDetales_MyEnteties_ParenId",
                        column: x => x.ParenId,
                        principalTable: "MyEnteties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyEnteties_ParentChatId",
                table: "MyEnteties",
                column: "ParentChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MyWorkDetales_ParenId",
                table: "MyWorkDetales",
                column: "ParenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyWorkDetales");

            migrationBuilder.DropTable(
                name: "MyEnteties");
        }
    }
}
