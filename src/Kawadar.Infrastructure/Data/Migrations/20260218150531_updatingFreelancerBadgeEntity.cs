using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatingFreelancerBadgeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges",
                column: "FreelancerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerBadges_UserProfiles_FreelancerId",
                table: "FreelancerBadges",
                column: "FreelancerId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerBadges_UserProfiles_FreelancerId",
                table: "FreelancerBadges");

            migrationBuilder.DropIndex(
                name: "IX_FreelancerBadges_FreelancerId",
                table: "FreelancerBadges");
        }
    }
}
