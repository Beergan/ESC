using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESC.CONCOST.Db.Migrations
{
    /// <inheritdoc />
    public partial class _20260511141033 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_LOGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    action_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    table_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    RecordGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    change_values = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOGS", x => x.Id);
                    table.UniqueConstraint("AK_AUDIT_LOGS_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    business_license = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ceo_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    approval_status = table.Column<int>(type: "int", nullable: false),
                    request_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reject_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_paid = table.Column<bool>(type: "bit", nullable: false),
                    membership_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CitizenID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education_Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfessionalQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LangId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE", x => x.Id);
                    table.UniqueConstraint("AK_EMPLOYEE_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "INDEX_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    index_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    index_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    data_source = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    update_freq = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_ppi_type = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INDEX_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SETTING_NOTIFICATION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guid_Notification = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Guid_User = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Guid_UserNotification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Check = table.Column<bool>(type: "bit", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETTING_NOTIFICATION", x => x.Id);
                    table.UniqueConstraint("AK_SETTING_NOTIFICATION_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "SETTING_PERMISSION",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETTING_PERMISSION", x => x.Id);
                    table.UniqueConstraint("AK_SETTING_PERMISSION_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuidEmployee = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeConnected = table.Column<bool>(type: "bit", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    approved_by = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    approved_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CONTRACTS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    project_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    contractor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    client = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    contract_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    bid_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    contract_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    contract_amount = table.Column<long>(type: "bigint", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    completion_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    bid_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    compare_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    adjust_no = table.Column<int>(type: "int", nullable: false),
                    advance_amt = table.Column<long>(type: "bigint", nullable: false),
                    excluded_amt = table.Column<long>(type: "bigint", nullable: false),
                    threshold_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    threshold_days = table.Column<int>(type: "int", nullable: false),
                    work_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    prepared_by = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRACTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONTRACTS_CUSTOMER_customer_id",
                        column: x => x.customer_id,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "INDEX_TIMESERIES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    period_key = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    index_key = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    index_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    data_verified = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INDEX_TIMESERIES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_INDEX_TIMESERIES_INDEX_TYPES_index_key",
                        column: x => x.index_key,
                        principalTable: "INDEX_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADJUST_RECORDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    contract_id = table.Column<int>(type: "int", nullable: false),
                    adjust_no = table.Column<int>(type: "int", nullable: false),
                    bid_date_used = table.Column<DateTime>(type: "datetime2", nullable: false),
                    compare_date_used = table.Column<DateTime>(type: "datetime2", nullable: false),
                    elapsed_days = table.Column<int>(type: "int", nullable: false),
                    kd_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    apply_amount = table.Column<long>(type: "bigint", nullable: false),
                    gross_adjust = table.Column<long>(type: "bigint", nullable: false),
                    advance_deduct = table.Column<long>(type: "bigint", nullable: false),
                    net_adjust = table.Column<long>(type: "bigint", nullable: false),
                    threshold_met = table.Column<bool>(type: "bit", nullable: false),
                    days_met = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADJUST_RECORDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ADJUST_RECORDS_CONTRACTS_contract_id",
                        column: x => x.contract_id,
                        principalTable: "CONTRACTS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CONTRACT_ITEMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    contract_id = table.Column<int>(type: "int", nullable: false),
                    item_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    group_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    item_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    index_key = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRACT_ITEMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONTRACT_ITEMS_CONTRACTS_contract_id",
                        column: x => x.contract_id,
                        principalTable: "CONTRACTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CONTRACT_ITEMS_INDEX_TYPES_index_key",
                        column: x => x.index_key,
                        principalTable: "INDEX_TYPES",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ESC_SERVICE_REQUEST",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    contract_id = table.Column<int>(type: "int", nullable: true),
                    request_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    admin_note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    attachment_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESC_SERVICE_REQUEST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ESC_SERVICE_REQUEST_CONTRACTS_contract_id",
                        column: x => x.contract_id,
                        principalTable: "CONTRACTS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ESC_SERVICE_REQUEST_CUSTOMER_customer_id",
                        column: x => x.customer_id,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ADJUST_ITEM_DETAILS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    record_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    index_key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    index0 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    index1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ki_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    wi_ki = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    is_manual = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADJUST_ITEM_DETAILS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ADJUST_ITEM_DETAILS_ADJUST_RECORDS_record_id",
                        column: x => x.record_id,
                        principalTable: "ADJUST_RECORDS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ADJUST_ITEM_DETAILS_CONTRACT_ITEMS_item_id",
                        column: x => x.item_id,
                        principalTable: "CONTRACT_ITEMS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADJUST_ITEM_DETAILS_item_id",
                table: "ADJUST_ITEM_DETAILS",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_ADJUST_ITEM_DETAILS_record_id",
                table: "ADJUST_ITEM_DETAILS",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "IX_ADJUST_RECORDS_contract_id",
                table: "ADJUST_RECORDS",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CustomerId",
                table: "AspNetUsers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACT_ITEMS_contract_id",
                table: "CONTRACT_ITEMS",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACT_ITEMS_index_key",
                table: "CONTRACT_ITEMS",
                column: "index_key");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACTS_customer_id",
                table: "CONTRACTS",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ESC_SERVICE_REQUEST_contract_id",
                table: "ESC_SERVICE_REQUEST",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_ESC_SERVICE_REQUEST_customer_id",
                table: "ESC_SERVICE_REQUEST",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_INDEX_TIMESERIES_index_key",
                table: "INDEX_TIMESERIES",
                column: "index_key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADJUST_ITEM_DETAILS");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AUDIT_LOGS");

            migrationBuilder.DropTable(
                name: "EMPLOYEE");

            migrationBuilder.DropTable(
                name: "ESC_SERVICE_REQUEST");

            migrationBuilder.DropTable(
                name: "INDEX_TIMESERIES");

            migrationBuilder.DropTable(
                name: "SETTING_NOTIFICATION");

            migrationBuilder.DropTable(
                name: "SETTING_PERMISSION");

            migrationBuilder.DropTable(
                name: "ADJUST_RECORDS");

            migrationBuilder.DropTable(
                name: "CONTRACT_ITEMS");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CONTRACTS");

            migrationBuilder.DropTable(
                name: "INDEX_TYPES");

            migrationBuilder.DropTable(
                name: "CUSTOMER");
        }
    }
}
