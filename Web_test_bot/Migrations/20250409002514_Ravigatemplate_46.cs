using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyPhotoId",
                table: "MyUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MyPhotoId",
                table: "MyDefoltUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MyPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    TypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    TypeDiscr = table.Column<string>(type: "TEXT", nullable: true),
                    IsDefoult = table.Column<bool>(type: "INTEGER", nullable: true),
                    ProcessId = table.Column<int>(type: "INTEGER", nullable: true),
                    MyMenuContentId = table.Column<int>(type: "INTEGER", nullable: true),
                    FileId = table.Column<string>(type: "TEXT", nullable: false),
                    FileUniqueId = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyPhoto_MyMenus_MyMenuContentId",
                        column: x => x.MyMenuContentId,
                        principalTable: "MyMenus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MyPhoto_MyProcesses_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "MyProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyUsers_MyPhotoId",
                table: "MyUsers",
                column: "MyPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_MyDefoltUsers_MyPhotoId",
                table: "MyDefoltUsers",
                column: "MyPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_MyMenuContentId",
                table: "MyPhoto",
                column: "MyMenuContentId");

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_ProcessId",
                table: "MyPhoto",
                column: "ProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyDefoltUsers_MyPhoto_MyPhotoId",
                table: "MyDefoltUsers",
                column: "MyPhotoId",
                principalTable: "MyPhoto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyUsers_MyPhoto_MyPhotoId",
                table: "MyUsers",
                column: "MyPhotoId",
                principalTable: "MyPhoto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyDefoltUsers_MyPhoto_MyPhotoId",
                table: "MyDefoltUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MyUsers_MyPhoto_MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropTable(
                name: "MyPhoto");

            migrationBuilder.DropIndex(
                name: "IX_MyUsers_MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropIndex(
                name: "IX_MyDefoltUsers_MyPhotoId",
                table: "MyDefoltUsers");

            migrationBuilder.DropColumn(
                name: "MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropColumn(
                name: "MyPhotoId",
                table: "MyDefoltUsers");
        }
    }
}
