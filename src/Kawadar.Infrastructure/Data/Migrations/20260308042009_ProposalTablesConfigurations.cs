using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProposalTablesConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobProposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreelancerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProposalType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedDays = table.Column<int>(type: "int", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedHours = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobProposals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProposalMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalMilestones_JobProposals_JobProposalId",
                        column: x => x.JobProposalId,
                        principalTable: "JobProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposalQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JobProposalId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalQuestionAnswers_JobProposals_JobProposalId",
                        column: x => x.JobProposalId,
                        principalTable: "JobProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProposalQuestionAnswers_JobProposals_JobProposalId1",
                        column: x => x.JobProposalId1,
                        principalTable: "JobProposals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProposalQuestionAnswers_JobQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "JobQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobProposals_JobId_FreelancerId",
                table: "JobProposals",
                columns: new[] { "JobId", "FreelancerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposalMilestones_JobProposalId",
                table: "ProposalMilestones",
                column: "JobProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalQuestionAnswers_JobProposalId_QuestionId",
                table: "ProposalQuestionAnswers",
                columns: new[] { "JobProposalId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposalQuestionAnswers_JobProposalId1",
                table: "ProposalQuestionAnswers",
                column: "JobProposalId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalQuestionAnswers_QuestionId",
                table: "ProposalQuestionAnswers",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalMilestones");

            migrationBuilder.DropTable(
                name: "ProposalQuestionAnswers");

            migrationBuilder.DropTable(
                name: "JobProposals");
        }
    }
}
