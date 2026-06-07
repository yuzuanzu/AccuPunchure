using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuPunchure.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Organizations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Employees",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Employees");
        }
    }
}
