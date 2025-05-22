using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyDefoltUsers_MyPhoto_MyPhotoId",
                table: "MyDefoltUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyProcesses_ProcessId",
                table: "MyPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_MyUsers_MyPhoto_MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropIndex(
                name: "IX_MyUsers_MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropIndex(
                name: "IX_MyPhoto_ProcessId",
                table: "MyPhoto");

            migrationBuilder.DropIndex(
                name: "IX_MyDefoltUsers_MyPhotoId",
                table: "MyDefoltUsers");

            migrationBuilder.DropColumn(
                name: "MyPhotoId",
                table: "MyUsers");

            migrationBuilder.DropColumn(
                name: "IsDefoult",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "TypeCode",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "TypeDiscr",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "MyPhotoId",
                table: "MyDefoltUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyPhotoId",
                table: "MyUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefoult",
                table: "MyPhoto",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessId",
                table: "MyPhoto",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeCode",
                table: "MyPhoto",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeDiscr",
                table: "MyPhoto",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "MyPhoto",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MyPhotoId",
                table: "MyDefoltUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyUsers_MyPhotoId",
                table: "MyUsers",
                column: "MyPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_ProcessId",
                table: "MyPhoto",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_MyDefoltUsers_MyPhotoId",
                table: "MyDefoltUsers",
                column: "MyPhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyDefoltUsers_MyPhoto_MyPhotoId",
                table: "MyDefoltUsers",
                column: "MyPhotoId",
                principalTable: "MyPhoto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyProcesses_ProcessId",
                table: "MyPhoto",
                column: "ProcessId",
                principalTable: "MyProcesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyUsers_MyPhoto_MyPhotoId",
                table: "MyUsers",
                column: "MyPhotoId",
                principalTable: "MyPhoto",
                principalColumn: "Id");
        }
    }
}
