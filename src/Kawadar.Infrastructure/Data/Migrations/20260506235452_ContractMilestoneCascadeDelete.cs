using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractMilestoneCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
