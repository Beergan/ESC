using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260512154642 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nameEn",
                table: "CONTRACT_CATEGORY",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "CONSTRUCTION_CATEGORY",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nameEn",
                table: "CONTRACT_CATEGORY");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "CONSTRUCTION_CATEGORY");
        }
    }
}
