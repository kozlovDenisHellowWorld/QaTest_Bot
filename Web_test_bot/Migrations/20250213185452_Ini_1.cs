using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_test_bot.Migrations
{
    /// <inheritdoc />
    public partial class Ini_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyIntupTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    TypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    TypeDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyIntupTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyMenuTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    TypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    TypeDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyMenuTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "myUserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: true),
                    TypeCode = table.Column<string>(type: "TEXT", nullable: false),
                    TypeDiscr = table.Column<string>(type: "TEXT", nullable: true),
                    IsDefoult = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_myUserTypes", x => x.Id);
                    table.UniqueConstraint("AK_myUserTypes_TypeCode", x => x.TypeCode);
                });

            migrationBuilder.CreateTable(
                name: "MyDefoltUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TelegramUserName = table.Column<string>(type: "TEXT", nullable: true),
                    TelegramId = table.Column<long>(type: "INTEGER", nullable: true),
                    UserTypeСode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDefoltUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyDefoltUsers_myUserTypes_UserTypeСode",
                        column: x => x.UserTypeСode,
                        principalTable: "myUserTypes",
                        principalColumn: "TypeCode");
                });

            migrationBuilder.CreateTable(
                name: "MyProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: true),
                    UserAccessCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyProcesses_myUserTypes_UserAccessCode",
                        column: x => x.UserAccessCode,
                        principalTable: "myUserTypes",
                        principalColumn: "TypeCode");
                });

            migrationBuilder.CreateTable(
                name: "MyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TeleId = table.Column<long>(type: "INTEGER", nullable: true),
                    usertypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TeleUserName = table.Column<string>(type: "TEXT", nullable: true),
                    TeleFirstName = table.Column<string>(type: "TEXT", nullable: true),
                    TeleLasttName = table.Column<string>(type: "TEXT", nullable: true),
                    TeleLanguageCode = table.Column<string>(type: "TEXT", nullable: true),
                    TeleIsBot = table.Column<bool>(type: "INTEGER", nullable: true),
                    CanJoinGroups = table.Column<bool>(type: "INTEGER", nullable: true),
                    CanReadAllGroupMessages = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsBan = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyUsers_myUserTypes_usertypeId",
                        column: x => x.usertypeId,
                        principalTable: "myUserTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MyMenuContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    MenuName = table.Column<string>(type: "TEXT", nullable: true),
                    MenuCode = table.Column<string>(type: "TEXT", nullable: false),
                    MenuTypeCode = table.Column<string>(type: "TEXT", nullable: true),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    NeedToDelete = table.Column<bool>(type: "INTEGER", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyMenuContent", x => x.Id);
                    table.UniqueConstraint("AK_MyMenuContent_MenuCode", x => x.MenuCode);
                    table.ForeignKey(
                        name: "FK_MyMenuContent_MyMenuTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "MyMenuTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MyMenuContent_MyProcesses_ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "MyProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MyChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TeleChatUserName = table.Column<string>(type: "TEXT", nullable: true),
                    TeleChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    TeleChatTitle = table.Column<string>(type: "TEXT", nullable: true),
                    TeleChatType = table.Column<int>(type: "INTEGER", nullable: true),
                    TeleChatMembersCount = table.Column<int>(type: "INTEGER", nullable: true),
                    IsGroupChat = table.Column<bool>(type: "INTEGER", nullable: true),
                    LastMessageDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsBan = table.Column<bool>(type: "INTEGER", nullable: true),
                    LastUpdateId = table.Column<int>(type: "INTEGER", nullable: true),
                    CurentMenuId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProcessCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyChats", x => x.Id);
                    table.UniqueConstraint("AK_MyChats_TeleChatId", x => x.TeleChatId);
                    table.ForeignKey(
                        name: "FK_MyChats_MyMenuContent_CurentMenuId",
                        column: x => x.CurentMenuId,
                        principalTable: "MyMenuContent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MyInput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    InputName = table.Column<string>(type: "TEXT", nullable: true),
                    InputDescription = table.Column<string>(type: "TEXT", nullable: true),
                    InputContent = table.Column<string>(type: "TEXT", nullable: true),
                    InputType = table.Column<string>(type: "TEXT", nullable: true),
                    NextMenuCode = table.Column<string>(type: "TEXT", nullable: true),
                    MenuId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyInput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyInput_MyMenuContent_MenuId",
                        column: x => x.MenuId,
                        principalTable: "MyMenuContent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MyInput_MyMenuContent_NextMenuCode",
                        column: x => x.NextMenuCode,
                        principalTable: "MyMenuContent",
                        principalColumn: "MenuCode");
                });

            migrationBuilder.CreateTable(
                name: "MyChatMyUser",
                columns: table => new
                {
                    ChatUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserGroupListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyChatMyUser", x => new { x.ChatUsersId, x.UserGroupListId });
                    table.ForeignKey(
                        name: "FK_MyChatMyUser_MyChats_UserGroupListId",
                        column: x => x.UserGroupListId,
                        principalTable: "MyChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MyChatMyUser_MyUsers_ChatUsersId",
                        column: x => x.ChatUsersId,
                        principalTable: "MyUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TeleChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageContent = table.Column<string>(type: "TEXT", nullable: true),
                    isComand = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsIncomingMessage = table.Column<bool>(type: "INTEGER", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TeleMessageId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyMessage_MyChats_TeleChatId",
                        column: x => x.TeleChatId,
                        principalTable: "MyChats",
                        principalColumn: "TeleChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDelite = table.Column<bool>(type: "INTEGER", nullable: true),
                    TeleUpdateId = table.Column<int>(type: "INTEGER", nullable: true),
                    TeleDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MessageText = table.Column<string>(type: "TEXT", nullable: true),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: true),
                    ChatTitle = table.Column<string>(type: "TEXT", nullable: true),
                    ChatType = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCommand = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsTextMessage = table.Column<bool>(type: "INTEGER", nullable: true),
                    Command = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: true),
                    IsInccoming = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyUpdates_MyChats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "MyChats",
                        principalColumn: "TeleChatId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyChatMyUser_UserGroupListId",
                table: "MyChatMyUser",
                column: "UserGroupListId");

            migrationBuilder.CreateIndex(
                name: "IX_MyChats_CurentMenuId",
                table: "MyChats",
                column: "CurentMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MyDefoltUsers_UserTypeСode",
                table: "MyDefoltUsers",
                column: "UserTypeСode");

            migrationBuilder.CreateIndex(
                name: "IX_MyInput_MenuId",
                table: "MyInput",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MyInput_NextMenuCode",
                table: "MyInput",
                column: "NextMenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_MyMenuContent_ProcessID",
                table: "MyMenuContent",
                column: "ProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_MyMenuContent_TypeId",
                table: "MyMenuContent",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MyMessage_TeleChatId",
                table: "MyMessage",
                column: "TeleChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MyProcesses_UserAccessCode",
                table: "MyProcesses",
                column: "UserAccessCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyUpdates_ChatId",
                table: "MyUpdates",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MyUsers_usertypeId",
                table: "MyUsers",
                column: "usertypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyChatMyUser");

            migrationBuilder.DropTable(
                name: "MyDefoltUsers");

            migrationBuilder.DropTable(
                name: "MyInput");

            migrationBuilder.DropTable(
                name: "MyIntupTypes");

            migrationBuilder.DropTable(
                name: "MyMessage");

            migrationBuilder.DropTable(
                name: "MyUpdates");

            migrationBuilder.DropTable(
                name: "MyUsers");

            migrationBuilder.DropTable(
                name: "MyChats");

            migrationBuilder.DropTable(
                name: "MyMenuContent");

            migrationBuilder.DropTable(
                name: "MyMenuTypes");

            migrationBuilder.DropTable(
                name: "MyProcesses");

            migrationBuilder.DropTable(
                name: "myUserTypes");
        }
    }
}
