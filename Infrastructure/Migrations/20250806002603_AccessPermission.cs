using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccessPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProjectPermissions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Security");

            migrationBuilder.DropColumn(
                name: "BusinessPhone",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Department",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OfficeLocation",
                schema: "Security",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MobilePhone",
                schema: "Security",
                table: "Users",
                newName: "SystemRole");

            migrationBuilder.AddColumn<string>(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ValuationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ValuationDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachedBy",
                schema: "Contracts",
                table: "MilestoneDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachedDate",
                schema: "Contracts",
                table: "MilestoneDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ContractDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ContractDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ValuationDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ValuationDocuments");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AttachedBy",
                schema: "Contracts",
                table: "MilestoneDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedDate",
                schema: "Contracts",
                table: "MilestoneDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedBy",
                schema: "Contracts",
                table: "ChangeOrderDocuments");

            migrationBuilder.DropColumn(
                name: "AttachedDate",
                schema: "Contracts",
                table: "ChangeOrderDocuments");

            migrationBuilder.RenameColumn(
                name: "SystemRole",
                schema: "Security",
                table: "Users",
                newName: "MobilePhone");

            migrationBuilder.AddColumn<string>(
                name: "BusinessPhone",
                schema: "Security",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                schema: "Security",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeLocation",
                schema: "Security",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProjectPermissions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjectPermissions", x => x.Id);
                    table.CheckConstraint("CK_UserProjectPermissions_ValidDates", "[ExpiresAt] IS NULL OR [ExpiresAt] > [GrantedAt]");
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Security",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                schema: "Security",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsDeleted",
                schema: "Security",
                table: "Permissions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Resource",
                schema: "Security",
                table: "Permissions",
                columns: new[] { "Module", "Resource" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_IsActive_GrantedAt_ExpiresAt",
                schema: "Security",
                table: "UserProjectPermissions",
                columns: new[] { "IsActive", "GrantedAt", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_PermissionId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_ProjectId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_UserId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_UserId_ProjectId_PermissionCode",
                schema: "Security",
                table: "UserProjectPermissions",
                columns: new[] { "UserId", "ProjectId", "PermissionCode" },
                unique: true);
        }
    }
}
