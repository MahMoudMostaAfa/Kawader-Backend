using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class configuringFreelancerBadgeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges",
                column: "FreelancerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_BadgeId",
                table: "FreelancerBadges");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges");

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
