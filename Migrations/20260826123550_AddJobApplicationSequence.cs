using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceptionSystem.Migrations
{
    public partial class AddJobApplicationSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE SEQUENCE dbo.JobApplicationSeq
                    START WITH 20
                    INCREMENT BY 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SEQUENCE dbo.JobApplicationSeq;");
        }
    }
}