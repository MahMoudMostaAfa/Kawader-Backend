using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawadar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addJobDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Specilizations_SpecilizationId",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_UserProfiles_PostedById",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_JobFile_Job_JobId",
                table: "JobFile");

            migrationBuilder.DropForeignKey(
                name: "FK_JobQuestion_Job_JobId",
                table: "JobQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Job_JobId",
                table: "JobSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobQuestion",
                table: "JobQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobFile",
                table: "JobFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Job",
                table: "Job");

            migrationBuilder.RenameTable(
                name: "JobQuestion",
                newName: "JobQuestions");

            migrationBuilder.RenameTable(
                name: "JobFile",
                newName: "JobFiles");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "Jobs");

            migrationBuilder.RenameIndex(
                name: "IX_JobQuestion_JobId",
                table: "JobQuestions",
                newName: "IX_JobQuestions_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobFile_JobId",
                table: "JobFiles",
                newName: "IX_JobFiles_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_SpecilizationId",
                table: "Jobs",
                newName: "IX_Jobs_SpecilizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_PostedById",
                table: "Jobs",
                newName: "IX_Jobs_PostedById");

            migrationBuilder.RenameIndex(
                name: "IX_Job_JobSlug",
                table: "Jobs",
                newName: "IX_Jobs_JobSlug");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobQuestions",
                table: "JobQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobFiles",
                table: "JobFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobFiles_Jobs_JobId",
                table: "JobFiles",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobQuestions_Jobs_JobId",
                table: "JobQuestions",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Specilizations_SpecilizationId",
                table: "Jobs",
                column: "SpecilizationId",
                principalTable: "Specilizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_UserProfiles_PostedById",
                table: "Jobs",
                column: "PostedById",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobFiles_Jobs_JobId",
                table: "JobFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JobQuestions_Jobs_JobId",
                table: "JobQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Specilizations_SpecilizationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_UserProfiles_PostedById",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Jobs_JobId",
                table: "JobSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobQuestions",
                table: "JobQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobFiles",
                table: "JobFiles");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "Job");

            migrationBuilder.RenameTable(
                name: "JobQuestions",
                newName: "JobQuestion");

            migrationBuilder.RenameTable(
                name: "JobFiles",
                newName: "JobFile");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_SpecilizationId",
                table: "Job",
                newName: "IX_Job_SpecilizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_PostedById",
                table: "Job",
                newName: "IX_Job_PostedById");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_JobSlug",
                table: "Job",
                newName: "IX_Job_JobSlug");

            migrationBuilder.RenameIndex(
                name: "IX_JobQuestions_JobId",
                table: "JobQuestion",
                newName: "IX_JobQuestion_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobFiles_JobId",
                table: "JobFile",
                newName: "IX_JobFile_JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Job",
                table: "Job",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobQuestion",
                table: "JobQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobFile",
                table: "JobFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Specilizations_SpecilizationId",
                table: "Job",
                column: "SpecilizationId",
                principalTable: "Specilizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_UserProfiles_PostedById",
                table: "Job",
                column: "PostedById",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobFile_Job_JobId",
                table: "JobFile",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobQuestion_Job_JobId",
                table: "JobQuestion",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Job_JobId",
                table: "JobSkills",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
