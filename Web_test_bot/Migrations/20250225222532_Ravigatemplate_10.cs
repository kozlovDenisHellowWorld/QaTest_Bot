using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurentMessageIdForMenu",
                table: "MyChats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurentTextModel",
                table: "MyChats",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurentMessageIdForMenu",
                table: "MyChats");

            migrationBuilder.DropColumn(
                name: "CurentTextModel",
                table: "MyChats");
        }
    }
}
