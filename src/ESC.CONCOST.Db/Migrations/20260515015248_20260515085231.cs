using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260515085231 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESC_FORMULA_FIELD_OPTIONS");

            migrationBuilder.DropTable(
                name: "ESC_FORMULA_FIELDS");

            migrationBuilder.DropColumn(
                name: "formula_expression",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.RenameColumn(
                name: "allow_negative_result",
                table: "ESC_FORMULA_SETTINGS",
                newName: "use_advance_deduction");

            migrationBuilder.AddColumn<string>(
                name: "adjustment_rate_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "advance_deduct_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "applicable_amount_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "composite_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "eligible_condition_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "final_adjustment_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gross_adjustment_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "index_ratio_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_current",
                table: "ESC_FORMULA_SETTINGS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "other_deduction_default",
                table: "ESC_FORMULA_SETTINGS",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "threshold_days",
                table: "ESC_FORMULA_SETTINGS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "threshold_rate",
                table: "ESC_FORMULA_SETTINGS",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "version_no",
                table: "ESC_FORMULA_SETTINGS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "weight_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "weighted_ratio_formula",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ESC_FORMULA_HISTORY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    formula_setting_id = table.Column<int>(type: "int", nullable: false),
                    formula_setting_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    version_no = table.Column<int>(type: "int", nullable: false),
                    snapshot_json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    change_note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_FORMULA_HISTORY", x => x.Id);
                    table.UniqueConstraint("AK_ESC_FORMULA_HISTORY_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ESC_FORMULA_HISTORY_ESC_FORMULA_SETTINGS_formula_setting_id",
                        column: x => x.formula_setting_id,
                        principalTable: "ESC_FORMULA_SETTINGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ESC_FORMULA_VARIABLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    variable_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    variable_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    variable_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    data_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    default_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    is_system = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_FORMULA_VARIABLES", x => x.Id);
                    table.UniqueConstraint("AK_ESC_FORMULA_VARIABLES_Guid", x => x.Guid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ESC_FORMULA_HISTORY_formula_setting_id",
                table: "ESC_FORMULA_HISTORY",
                column: "formula_setting_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESC_FORMULA_HISTORY");

            migrationBuilder.DropTable(
                name: "ESC_FORMULA_VARIABLES");

            migrationBuilder.DropColumn(
                name: "adjustment_rate_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "advance_deduct_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "applicable_amount_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "composite_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "eligible_condition_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "final_adjustment_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "gross_adjustment_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "index_ratio_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "is_current",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "other_deduction_default",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "threshold_days",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "threshold_rate",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "version_no",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "weight_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "weighted_ratio_formula",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.RenameColumn(
                name: "use_advance_deduction",
                table: "ESC_FORMULA_SETTINGS",
                newName: "allow_negative_result");

            migrationBuilder.AddColumn<string>(
                name: "formula_expression",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ESC_FORMULA_FIELDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    formula_setting_id = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    default_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    field_key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    field_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_readonly = table.Column<bool>(type: "bit", nullable: false),
                    is_required = table.Column<bool>(type: "bit", nullable: false),
                    label_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    label_ko = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    placeholder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    use_in_formula = table.Column<bool>(type: "bit", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    validation_max = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    validation_min = table.Column<decimal>(type: "decimal(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_FORMULA_FIELDS", x => x.Id);
                    table.UniqueConstraint("AK_ESC_FORMULA_FIELDS_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ESC_FORMULA_FIELDS_ESC_FORMULA_SETTINGS_formula_setting_id",
                        column: x => x.formula_setting_id,
                        principalTable: "ESC_FORMULA_SETTINGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ESC_FORMULA_FIELD_OPTIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    formula_field_id = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    option_text_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    option_text_ko = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    option_value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_FORMULA_FIELD_OPTIONS", x => x.Id);
                    table.UniqueConstraint("AK_ESC_FORMULA_FIELD_OPTIONS_Guid", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ESC_FORMULA_FIELD_OPTIONS_ESC_FORMULA_FIELDS_formula_field_id",
                        column: x => x.formula_field_id,
                        principalTable: "ESC_FORMULA_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ESC_FORMULA_FIELD_OPTIONS_formula_field_id",
                table: "ESC_FORMULA_FIELD_OPTIONS",
                column: "formula_field_id");

            migrationBuilder.CreateIndex(
                name: "IX_ESC_FORMULA_FIELDS_formula_setting_id",
                table: "ESC_FORMULA_FIELDS",
                column: "formula_setting_id");
        }
    }
}
