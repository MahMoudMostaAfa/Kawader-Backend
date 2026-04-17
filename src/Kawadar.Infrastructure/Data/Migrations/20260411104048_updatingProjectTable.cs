using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatingProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PortfolioProjects");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecilizationId",
                table: "PortfolioProjects",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioProjects_SpecilizationId",
                table: "PortfolioProjects",
                column: "SpecilizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioProjects_Specilizations_SpecilizationId",
                table: "PortfolioProjects",
                column: "SpecilizationId",
                principalTable: "Specilizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioProjects_Specilizations_SpecilizationId",
                table: "PortfolioProjects");

            migrationBuilder.DropIndex(
                name: "IX_PortfolioProjects_SpecilizationId",
                table: "PortfolioProjects");

            migrationBuilder.DropColumn(
                name: "SpecilizationId",
                table: "PortfolioProjects");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PortfolioProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
