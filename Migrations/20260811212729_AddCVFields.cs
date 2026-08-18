using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceptionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCVFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvContentType",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CvFileData",
                table: "JobApplications",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvFileName",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvContentType",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CvFileData",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CvFileName",
                table: "JobApplications");
        }
    }
}
