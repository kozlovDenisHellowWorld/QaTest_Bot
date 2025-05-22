using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_25 : Migration
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
                name: "FK_MyMenuContent_MyMenuTypes_TypeId",
                table: "MyMenuContent");

            migrationBuilder.DropForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MyMenuContent_MenuCode",
                table: "MyMenuContent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyMenuContent",
                table: "MyMenuContent");

            migrationBuilder.RenameTable(
                name: "MyMenuContent",
                newName: "MyMenus");

            migrationBuilder.RenameIndex(
                name: "IX_MyMenuContent_TypeId",
                table: "MyMenus",
                newName: "IX_MyMenus_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MyMenuContent_ProcessID",
                table: "MyMenus",
                newName: "IX_MyMenus_ProcessID");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MyMenus_MenuCode",
                table: "MyMenus",
                column: "MenuCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyMenus",
                table: "MyMenus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyChats_MyMenus_CurentMenuId",
                table: "MyChats",
                column: "CurentMenuId",
                principalTable: "MyMenus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenus_MenuId",
                table: "MyInput",
                column: "MenuId",
                principalTable: "MyMenus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyInput_MyMenus_NextMenuCode",
                table: "MyInput",
                column: "NextMenuCode",
                principalTable: "MyMenus",
                principalColumn: "MenuCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyMenus_MyMenuTypes_TypeId",
                table: "MyMenus",
                column: "TypeId",
                principalTable: "MyMenuTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyMenus_MyProcesses_ProcessID",
                table: "MyMenus",
                column: "ProcessID",
                principalTable: "MyProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyChats_MyMenus_CurentMenuId",
                table: "MyChats");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenus_MenuId",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyInput_MyMenus_NextMenuCode",
                table: "MyInput");

            migrationBuilder.DropForeignKey(
                name: "FK_MyMenus_MyMenuTypes_TypeId",
                table: "MyMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_MyMenus_MyProcesses_ProcessID",
                table: "MyMenus");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MyMenus_MenuCode",
                table: "MyMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyMenus",
                table: "MyMenus");

            migrationBuilder.RenameTable(
                name: "MyMenus",
                newName: "MyMenuContent");

            migrationBuilder.RenameIndex(
                name: "IX_MyMenus_TypeId",
                table: "MyMenuContent",
                newName: "IX_MyMenuContent_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MyMenus_ProcessID",
                table: "MyMenuContent",
                newName: "IX_MyMenuContent_ProcessID");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MyMenuContent_MenuCode",
                table: "MyMenuContent",
                column: "MenuCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyMenuContent",
                table: "MyMenuContent",
                column: "Id");

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
                name: "FK_MyMenuContent_MyMenuTypes_TypeId",
                table: "MyMenuContent",
                column: "TypeId",
                principalTable: "MyMenuTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyMenuContent_MyProcesses_ProcessID",
                table: "MyMenuContent",
                column: "ProcessID",
                principalTable: "MyProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
