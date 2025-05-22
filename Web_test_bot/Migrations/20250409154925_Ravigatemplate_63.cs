using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_63 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto");

            migrationBuilder.AddColumn<int>(
                name: "ParentMenuID",
                table: "MyPhoto",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyPhoto_ParentMenuID",
                table: "MyPhoto",
                column: "ParentMenuID");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyMessage_ParentMenuID",
                table: "MyPhoto",
                column: "ParentMenuID",
                principalTable: "MyMessage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto",
                column: "MyWorkDetalesId",
                principalTable: "MyWorkDetales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyMessage_ParentMenuID",
                table: "MyPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto");

            migrationBuilder.DropIndex(
                name: "IX_MyPhoto_ParentMenuID",
                table: "MyPhoto");

            migrationBuilder.DropColumn(
                name: "ParentMenuID",
                table: "MyPhoto");

            migrationBuilder.AddForeignKey(
                name: "FK_MyPhoto_MyWorkDetales_MyWorkDetalesId",
                table: "MyPhoto",
                column: "MyWorkDetalesId",
                principalTable: "MyWorkDetales",
                principalColumn: "Id");
        }
    }
}
