using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Solutions.Portal.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddJobCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobCategory",
                table: "Inquiries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "Inquiries");
        }
    }
}
