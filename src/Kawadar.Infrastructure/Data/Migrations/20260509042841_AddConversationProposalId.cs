using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationProposalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentEventHook_PaymentTransaction_PaymentTransactionId",
                table: "PaymentEventHook");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_UserProfiles_UserId",
                table: "PaymentTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_WalletTransactions_WalletTransactionId",
                table: "PaymentTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_Wallets_WalletId",
                table: "PaymentTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransaction",
                table: "PaymentTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentEventHook",
                table: "PaymentEventHook");

            migrationBuilder.RenameTable(
                name: "PaymentTransaction",
                newName: "PaymentTransactions");

            migrationBuilder.RenameTable(
                name: "PaymentEventHook",
                newName: "PaymentEventHooks");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransaction_WalletTransactionId",
                table: "PaymentTransactions",
                newName: "IX_PaymentTransactions_WalletTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransaction_WalletId",
                table: "PaymentTransactions",
                newName: "IX_PaymentTransactions_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransaction_UserId",
                table: "PaymentTransactions",
                newName: "IX_PaymentTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentEventHook_PaymentTransactionId",
                table: "PaymentEventHooks",
                newName: "IX_PaymentEventHooks_PaymentTransactionId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalId1",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentEventHooks",
                table: "PaymentEventHooks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ProposalId",
                table: "Conversations",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ProposalId1",
                table: "Conversations",
                column: "ProposalId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_JobProposals_ProposalId",
                table: "Conversations",
                column: "ProposalId",
                principalTable: "JobProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_JobProposals_ProposalId1",
                table: "Conversations",
                column: "ProposalId1",
                principalTable: "JobProposals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentEventHooks_PaymentTransactions_PaymentTransactionId",
                table: "PaymentEventHooks",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_UserProfiles_UserId",
                table: "PaymentTransactions",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_WalletTransactions_WalletTransactionId",
                table: "PaymentTransactions",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Wallets_WalletId",
                table: "PaymentTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_JobProposals_ProposalId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_JobProposals_ProposalId1",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentEventHooks_PaymentTransactions_PaymentTransactionId",
                table: "PaymentEventHooks");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_UserProfiles_UserId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_WalletTransactions_WalletTransactionId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Wallets_WalletId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ProposalId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ProposalId1",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentEventHooks",
                table: "PaymentEventHooks");

            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ProposalId1",
                table: "Conversations");

            migrationBuilder.RenameTable(
                name: "PaymentTransactions",
                newName: "PaymentTransaction");

            migrationBuilder.RenameTable(
                name: "PaymentEventHooks",
                newName: "PaymentEventHook");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransactions_WalletTransactionId",
                table: "PaymentTransaction",
                newName: "IX_PaymentTransaction_WalletTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransactions_WalletId",
                table: "PaymentTransaction",
                newName: "IX_PaymentTransaction_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransaction",
                newName: "IX_PaymentTransaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentEventHooks_PaymentTransactionId",
                table: "PaymentEventHook",
                newName: "IX_PaymentEventHook_PaymentTransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransaction",
                table: "PaymentTransaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentEventHook",
                table: "PaymentEventHook",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentEventHook_PaymentTransaction_PaymentTransactionId",
                table: "PaymentEventHook",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_UserProfiles_UserId",
                table: "PaymentTransaction",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_WalletTransactions_WalletTransactionId",
                table: "PaymentTransaction",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_Wallets_WalletId",
                table: "PaymentTransaction",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }
    }
}
