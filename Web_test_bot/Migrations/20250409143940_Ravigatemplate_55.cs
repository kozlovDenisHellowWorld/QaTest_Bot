using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyWorkDetalesId",
                table: "MyPhoto",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_MyWorkDetalesId",
                table: "MyPhoto",
                column: "MyWorkDetalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto",
                column: "MyWorkDetalesId",
                principalTable: "MyWorkDetales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto");

            migrationBuilder.DropIndex(
                name: "IX_MyPhoto_MyWorkDetalesId",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "MyWorkDetalesId",
                table: "MyPhoto");
        }
    }
}
