using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingSkillAndRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectViews_PortfolioProjectId",
                table: "ProjectViews");

            migrationBuilder.DropIndex(
                name: "IX_ProjectViews_UserProfileId",
                table: "ProjectViews");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges");

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_UserProfiles_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreelacnerSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreelancerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SkillType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomSkillName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelacnerSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreelacnerSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreelacnerSkills_UserProfiles_FreelancerId",
                        column: x => x.FreelancerId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_PortfolioProjectId",
                table: "ProjectViews",
                column: "PortfolioProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_UserProfileId",
                table: "ProjectViews",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_SkillId",
                table: "ProjectSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelacnerSkills_FreelancerId",
                table: "FreelacnerSkills",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelacnerSkills_SkillId",
                table: "FreelacnerSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_CreatedBy",
                table: "Skills",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSkills_Skills_SkillId",
                table: "ProjectSkills");

            migrationBuilder.DropTable(
                name: "FreelacnerSkills");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_ProjectViews_PortfolioProjectId",
                table: "ProjectViews");

            migrationBuilder.DropIndex(
                name: "IX_ProjectViews_UserProfileId",
                table: "ProjectViews");

            migrationBuilder.DropIndex(
                name: "IX_ProjectSkills_SkillId",
                table: "ProjectSkills");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_PortfolioProjectId",
                table: "ProjectViews",
                column: "PortfolioProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_UserProfileId",
                table: "ProjectViews",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges",
                column: "BadgeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges",
                column: "FreelancerId",
                unique: true);
        }
    }
}
