using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyChats_MyMenuContent_CurentMenuId",
                table: "MyChats");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenuContent_MenuId",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenuContent_NextMenuCode",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent");

            migrationBuilder.AddForeignKey(
                name: "FK_MyChats_MyMenuContent_CurentMenuId",
                table: "MyChats",
                column: "CurentMenuId",
                principalTable: "MyMenuContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenuContent_MenuId",
                table: "MyInput",
                column: "MenuId",
                principalTable: "MyMenuContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenuContent_NextMenuCode",
                table: "MyInput",
                column: "NextMenuCode",
                principalTable: "MyMenuContent",
                principalColumn: "MenuCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent",
                column: "ProcessID",
                principalTable: "MyProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyChats_MyMenuContent_CurentMenuId",
                table: "MyChats");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenuContent_MenuId",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenuContent_NextMenuCode",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent");

            migrationBuilder.AddForeignKey(
                name: "FK_MyChats_MyMenuContent_CurentMenuId",
                table: "MyChats",
                column: "CurentMenuId",
                principalTable: "MyMenuContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenuContent_MenuId",
                table: "MyInput",
                column: "MenuId",
                principalTable: "MyMenuContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenuContent_NextMenuCode",
                table: "MyInput",
                column: "NextMenuCode",
                principalTable: "MyMenuContent",
                principalColumn: "MenuCode");

            migrationBuilder.AddForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent",
                column: "ProcessID",
                principalTable: "MyProcesses",
                principalColumn: "Id");
        }
    }
}
