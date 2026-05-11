using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrivateUserIdToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PrivateToUserId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PrivateToUserId",
                table: "Jobs",
                column: "PrivateToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_UserProfiles_PrivateToUserId",
                table: "Jobs",
                column: "PrivateToUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_UserProfiles_PrivateToUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_PrivateToUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PrivateToUserId",
                table: "Jobs");
        }
    }
}
