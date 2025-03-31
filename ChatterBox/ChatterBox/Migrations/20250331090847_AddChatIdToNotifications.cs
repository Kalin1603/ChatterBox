﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatterBox.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ChatId",
                table: "Notifications",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Chats_ChatId",
                table: "Notifications",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Chats_ChatId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ChatId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Notifications");
        }
    }
}
