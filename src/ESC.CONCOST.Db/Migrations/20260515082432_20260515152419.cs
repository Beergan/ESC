using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260515152419 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "index_group_id",
                table: "INDEX_TYPES",
                type: "int",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_INDEX_TYPES_Guid",
                table: "INDEX_TYPES",
                column: "Guid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_INDEX_TIMESERIES_Guid",
                table: "INDEX_TIMESERIES",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "ESC_INDEX_GROUPS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    group_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    group_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    group_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    icon_class = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_INDEX_GROUPS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_INDEX_TYPES_index_group_id",
                table: "INDEX_TYPES",
                column: "index_group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_INDEX_TYPES_ESC_INDEX_GROUPS_index_group_id",
                table: "INDEX_TYPES",
                column: "index_group_id",
                principalTable: "ESC_INDEX_GROUPS",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_INDEX_TYPES_ESC_INDEX_GROUPS_index_group_id",
                table: "INDEX_TYPES");

            migrationBuilder.DropTable(
                name: "ESC_INDEX_GROUPS");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_INDEX_TYPES_Guid",
                table: "INDEX_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_INDEX_TYPES_index_group_id",
                table: "INDEX_TYPES");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_INDEX_TIMESERIES_Guid",
                table: "INDEX_TIMESERIES");

            migrationBuilder.DropColumn(
                name: "index_group_id",
                table: "INDEX_TYPES");
        }
    }
}
