using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_52 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyMessageId",
                table: "MyPhoto",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_MyMessageId",
                table: "MyPhoto",
                column: "MyMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyMessage_MyMessageId",
                table: "MyPhoto",
                column: "MyMessageId",
                principalTable: "MyMessage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyMessage_MyMessageId",
                table: "MyPhoto");

            migrationBuilder.DropIndex(
                name: "IX_MyPhoto_MyMessageId",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "MyMessageId",
                table: "MyPhoto");
        }
    }
}
