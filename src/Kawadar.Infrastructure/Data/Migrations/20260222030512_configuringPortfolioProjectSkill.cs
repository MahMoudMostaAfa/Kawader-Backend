using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class configuringPortfolioProjectSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectSkills_PortfolioProjectId",
                table: "ProjectSkills");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_PortfolioProjectId",
                table: "ProjectSkills",
                column: "PortfolioProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectSkills_PortfolioProjectId",
                table: "ProjectSkills");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_PortfolioProjectId",
                table: "ProjectSkills",
                column: "PortfolioProjectId",
                unique: true);
        }
    }
}
