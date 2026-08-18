using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReceptionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDrivingLicenseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "JobApplications");

            migrationBuilder.AddColumn<int>(
                name: "DrivingLicenseTypeId",
                table: "JobApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DrivingLicenseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrivingLicenseTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DrivingLicenseTypes",
                columns: new[] { "Id", "Category" },
                values: new object[,]
                {
                    { 1, "A" },
                    { 2, "B" },
                    { 3, "C" },
                    { 4, "D" },
                    { 5, "D1" },
                    { 6, "D2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_DrivingLicenseTypeId",
                table: "JobApplications",
                column: "DrivingLicenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_DrivingLicenseTypes_DrivingLicenseTypeId",
                table: "JobApplications",
                column: "DrivingLicenseTypeId",
                principalTable: "DrivingLicenseTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_DrivingLicenseTypes_DrivingLicenseTypeId",
                table: "JobApplications");

            migrationBuilder.DropTable(
                name: "DrivingLicenseTypes");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_DrivingLicenseTypeId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseTypeId",
                table: "JobApplications");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationStatus",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
