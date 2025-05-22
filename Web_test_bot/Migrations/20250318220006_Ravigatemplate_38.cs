using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ravigatemplate_38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ParenId",
                table: "MyWorkDetales",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "CodeType",
                table: "MyEnteties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDone",
                table: "MyEnteties",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeType",
                table: "MyEnteties");

            migrationBuilder.DropColumn(
                name: "isDone",
                table: "MyEnteties");

            migrationBuilder.AlterColumn<int>(
                name: "ParenId",
                table: "MyWorkDetales",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
