using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260514160123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ESC_FORMULA_SETTINGS_ESC_FORMULA_SETTINGS_parent_formula_id",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropIndex(
                name: "IX_ESC_FORMULA_SETTINGS_parent_formula_id",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "allow_decrease_adjustment",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "parent_formula_id",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "threshold_rate",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.DropColumn(
                name: "use_advance_deduction",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "ESC_FORMULA_SETTINGS",
                newName: "sort_order");

            migrationBuilder.RenameColumn(
                name: "use_other_deduction",
                table: "ESC_FORMULA_SETTINGS",
                newName: "allow_negative_result");

            migrationBuilder.RenameColumn(
                name: "threshold_days",
                table: "ESC_FORMULA_SETTINGS",
                newName: "decimal_places");

            migrationBuilder.RenameColumn(
                name: "name_ko",
                table: "ESC_FORMULA_SETTINGS",
                newName: "formula_name_en");

            migrationBuilder.RenameColumn(
                name: "name_en",
                table: "ESC_FORMULA_SETTINGS",
                newName: "formula_name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "ESC_FORMULA_SETTINGS",
                newName: "formula_code");

            migrationBuilder.AlterColumn<string>(
                name: "formula_expression",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(1000)",
                maxLength: 1000,
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
                    field_key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    label_ko = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    label_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    field_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    default_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    placeholder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_required = table.Column<bool>(type: "bit", nullable: false),
                    is_readonly = table.Column<bool>(type: "bit", nullable: false),
                    use_in_formula = table.Column<bool>(type: "bit", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    validation_min = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    validation_max = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    option_value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    option_text_ko = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    option_text_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESC_FORMULA_FIELD_OPTIONS");

            migrationBuilder.DropTable(
                name: "ESC_FORMULA_FIELDS");

            migrationBuilder.DropColumn(
                name: "description",
                table: "ESC_FORMULA_SETTINGS");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "ESC_FORMULA_SETTINGS",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "formula_name_en",
                table: "ESC_FORMULA_SETTINGS",
                newName: "name_ko");

            migrationBuilder.RenameColumn(
                name: "formula_name",
                table: "ESC_FORMULA_SETTINGS",
                newName: "name_en");

            migrationBuilder.RenameColumn(
                name: "formula_code",
                table: "ESC_FORMULA_SETTINGS",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "decimal_places",
                table: "ESC_FORMULA_SETTINGS",
                newName: "threshold_days");

            migrationBuilder.RenameColumn(
                name: "allow_negative_result",
                table: "ESC_FORMULA_SETTINGS",
                newName: "use_other_deduction");

            migrationBuilder.AlterColumn<string>(
                name: "formula_expression",
                table: "ESC_FORMULA_SETTINGS",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddColumn<bool>(
                name: "allow_decrease_adjustment",
                table: "ESC_FORMULA_SETTINGS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "parent_formula_id",
                table: "ESC_FORMULA_SETTINGS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "threshold_rate",
                table: "ESC_FORMULA_SETTINGS",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "use_advance_deduction",
                table: "ESC_FORMULA_SETTINGS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ESC_FORMULA_SETTINGS_parent_formula_id",
                table: "ESC_FORMULA_SETTINGS",
                column: "parent_formula_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ESC_FORMULA_SETTINGS_ESC_FORMULA_SETTINGS_parent_formula_id",
                table: "ESC_FORMULA_SETTINGS",
                column: "parent_formula_id",
                principalTable: "ESC_FORMULA_SETTINGS",
                principalColumn: "Id");
        }
    }
}
