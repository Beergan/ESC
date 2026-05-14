using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260514153215 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "group_code",
                table: "INDEX_TYPES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "index_code",
                table: "INDEX_TYPES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "index_name_en",
                table: "INDEX_TYPES",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "INDEX_TYPES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "INDEX_TYPES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "index_value",
                table: "INDEX_TIMESERIES",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "month",
                table: "INDEX_TIMESERIES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "year",
                table: "INDEX_TIMESERIES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ESC_FORMULA_SETTINGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name_ko = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    formula_expression = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    threshold_rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    threshold_days = table.Column<int>(type: "int", nullable: false),
                    rounding_method = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    use_advance_deduction = table.Column<bool>(type: "bit", nullable: false),
                    use_other_deduction = table.Column<bool>(type: "bit", nullable: false),
                    allow_decrease_adjustment = table.Column<bool>(type: "bit", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    parent_formula_id = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_FORMULA_SETTINGS", x => x.Id);
                    table.UniqueConstraint("AK_ESC_FORMULA_SETTINGS_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ESC_FORMULA_SETTINGS_ESC_FORMULA_SETTINGS_parent_formula_id",
                        column: x => x.parent_formula_id,
                        principalTable: "ESC_FORMULA_SETTINGS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ESC_FORMULA_SETTINGS_parent_formula_id",
                table: "ESC_FORMULA_SETTINGS",
                column: "parent_formula_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "group_code",
                table: "INDEX_TYPES");

            migrationBuilder.DropColumn(
                name: "index_code",
                table: "INDEX_TYPES");

            migrationBuilder.DropColumn(
                name: "index_name_en",
                table: "INDEX_TYPES");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "INDEX_TYPES");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "INDEX_TYPES");

            migrationBuilder.DropColumn(
                name: "month",
                table: "INDEX_TIMESERIES");

            migrationBuilder.DropColumn(
                name: "year",
                table: "INDEX_TIMESERIES");

            migrationBuilder.AlterColumn<decimal>(
                name: "index_value",
                table: "INDEX_TIMESERIES",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");
        }
    }
}
