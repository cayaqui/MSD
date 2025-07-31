using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Setup");

            migrationBuilder.EnsureSchema(
                name: "UIUX");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "Setup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    FiscalYearStart = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntraId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginCount = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                schema: "Setup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ManagerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Setup",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "Setup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCharter = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Objectives = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Deliverables = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Client = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Setup",
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "UIUX",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MetadataJson = table.Column<string>(type: "ntext", nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.CheckConstraint("CK_Notification_ExpiresAt", "ExpiresAt >= CreatedAt");
                    table.CheckConstraint("CK_Notification_ReadDate", "ReadAt >= CreatedAt");
                    table.ForeignKey(
                        name: "FK_Notifications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Setup",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Setup",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeamMembers",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTeamMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Setup",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTeamMembers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Code",
                schema: "Setup",
                table: "Companies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsDeleted",
                schema: "Setup",
                table: "Companies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TaxId",
                schema: "Setup",
                table: "Companies",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                schema: "UIUX",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                schema: "UIUX",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExpiresAt",
                schema: "UIUX",
                table: "Notifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsDeleted_Status",
                schema: "UIUX",
                table: "Notifications",
                columns: new[] { "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Priority",
                schema: "UIUX",
                table: "Notifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProjectId",
                schema: "UIUX",
                table: "Notifications",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                schema: "UIUX",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                schema: "UIUX",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_User_Status_Date",
                schema: "UIUX",
                table: "Notifications",
                columns: new[] { "UserId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "UIUX",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsImportant",
                schema: "UIUX",
                table: "Notifications",
                columns: new[] { "UserId", "IsImportant" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Status",
                schema: "UIUX",
                table: "Notifications",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CompanyId_Code",
                schema: "Setup",
                table: "Operations",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_IsDeleted",
                schema: "Setup",
                table: "Operations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Code",
                schema: "Setup",
                table: "Projects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDeleted_IsActive",
                schema: "Setup",
                table: "Projects",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OperationId_Status",
                schema: "Setup",
                table: "Projects",
                columns: new[] { "OperationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_WBSCode",
                schema: "Setup",
                table: "Projects",
                column: "WBSCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_ProjectId_UserId_Role",
                schema: "Security",
                table: "ProjectTeamMembers",
                columns: new[] { "ProjectId", "UserId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_UserId_IsActive",
                schema: "Security",
                table: "ProjectTeamMembers",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Security",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EntraId",
                schema: "Security",
                table: "Users",
                column: "EntraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted_IsActive",
                schema: "Security",
                table: "Users",
                columns: new[] { "IsDeleted", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "UIUX");

            migrationBuilder.DropTable(
                name: "ProjectTeamMembers",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Setup");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Operations",
                schema: "Setup");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "Setup");
        }
    }
}
