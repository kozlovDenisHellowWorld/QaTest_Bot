using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriviousIncummingMessageId",
                table: "MyChats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyChats_PriviousIncummingMessageId",
                table: "MyChats",
                column: "PriviousIncummingMessageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MyChats_MyMessage_PriviousIncummingMessageId",
                table: "MyChats",
                column: "PriviousIncummingMessageId",
                principalTable: "MyMessage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyChats_MyMessage_PriviousIncummingMessageId",
                table: "MyChats");

            migrationBuilder.DropIndex(
                name: "IX_MyChats_PriviousIncummingMessageId",
                table: "MyChats");

            migrationBuilder.DropColumn(
                name: "PriviousIncummingMessageId",
                table: "MyChats");
        }
    }
}
