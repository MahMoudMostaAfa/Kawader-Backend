using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditContractTablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_JobProposals_ProposalId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Jobs_JobId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_UserProfiles_ClientId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_UserProfiles_FreelancerId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestone_Contract_ContractId",
                table: "ContractMilestone");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestone_ProposalMilestones_ProposalMilestoneId",
                table: "ContractMilestone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractMilestone",
                table: "ContractMilestone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contract",
                table: "Contract");

            migrationBuilder.RenameTable(
                name: "ContractMilestone",
                newName: "ContractMilestones");

            migrationBuilder.RenameTable(
                name: "Contract",
                newName: "Contracts");

            migrationBuilder.RenameIndex(
                name: "IX_ContractMilestone_ProposalMilestoneId",
                table: "ContractMilestones",
                newName: "IX_ContractMilestones_ProposalMilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractMilestone_ContractId",
                table: "ContractMilestones",
                newName: "IX_ContractMilestones_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_ProposalId",
                table: "Contracts",
                newName: "IX_Contracts_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_JobId",
                table: "Contracts",
                newName: "IX_Contracts_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_FreelancerId",
                table: "Contracts",
                newName: "IX_Contracts_FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_ClientId",
                table: "Contracts",
                newName: "IX_Contracts_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractMilestones",
                table: "ContractMilestones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestones_ProposalMilestones_ProposalMilestoneId",
                table: "ContractMilestones",
                column: "ProposalMilestoneId",
                principalTable: "ProposalMilestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_JobProposals_ProposalId",
                table: "Contracts",
                column: "ProposalId",
                principalTable: "JobProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Jobs_JobId",
                table: "Contracts",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_UserProfiles_ClientId",
                table: "Contracts",
                column: "ClientId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_UserProfiles_FreelancerId",
                table: "Contracts",
                column: "FreelancerId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestones_Contracts_ContractId",
                table: "ContractMilestones");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractMilestones_ProposalMilestones_ProposalMilestoneId",
                table: "ContractMilestones");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_JobProposals_ProposalId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Jobs_JobId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_UserProfiles_ClientId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_UserProfiles_FreelancerId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractMilestones",
                table: "ContractMilestones");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contract");

            migrationBuilder.RenameTable(
                name: "ContractMilestones",
                newName: "ContractMilestone");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ProposalId",
                table: "Contract",
                newName: "IX_Contract_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_JobId",
                table: "Contract",
                newName: "IX_Contract_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_FreelancerId",
                table: "Contract",
                newName: "IX_Contract_FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ClientId",
                table: "Contract",
                newName: "IX_Contract_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractMilestones_ProposalMilestoneId",
                table: "ContractMilestone",
                newName: "IX_ContractMilestone_ProposalMilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractMilestones_ContractId",
                table: "ContractMilestone",
                newName: "IX_ContractMilestone_ContractId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contract",
                table: "Contract",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractMilestone",
                table: "ContractMilestone",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_JobProposals_ProposalId",
                table: "Contract",
                column: "ProposalId",
                principalTable: "JobProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Jobs_JobId",
                table: "Contract",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_UserProfiles_ClientId",
                table: "Contract",
                column: "ClientId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_UserProfiles_FreelancerId",
                table: "Contract",
                column: "FreelancerId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestone_Contract_ContractId",
                table: "ContractMilestone",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMilestone_ProposalMilestones_ProposalMilestoneId",
                table: "ContractMilestone",
                column: "ProposalMilestoneId",
                principalTable: "ProposalMilestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
