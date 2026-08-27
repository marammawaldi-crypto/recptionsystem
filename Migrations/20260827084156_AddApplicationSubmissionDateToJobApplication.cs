using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceptionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationSubmissionDateToJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationSubmissionDate",
                table: "JobApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationSubmissionDate",
                table: "JobApplications");
        }
    }
}
