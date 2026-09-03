using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantScopedAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionsId",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                schema: "catalog",
                table: "Products");

            migrationBuilder.EnsureSchema(
                name: "tenancy");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                schema: "identity",
                table: "RolePermissions",
                newName: "PermissionId");

            migrationBuilder.AddColumn<Guid>(
                name: "SecurityStamp",
                schema: "identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<int>(
                name: "AllowedClients",
                schema: "identity",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemRole",
                schema: "identity",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "identity",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationPath",
                schema: "identity",
                table: "Roles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                schema: "identity",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "ClientType",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Web");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIp",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                schema: "identity",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<bool>(
                name: "IsImpersonating",
                schema: "identity",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "identity",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "RevokedReason",
                schema: "identity",
                table: "RefreshTokens",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "catalog",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganizationPath",
                schema: "catalog",
                table: "Products",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPlatformOnly",
                schema: "identity",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxScope",
                schema: "identity",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Module",
                schema: "identity",
                table: "Permissions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientType",
                schema: "identity",
                table: "LoginAttempts",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "identity",
                table: "LoginAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "catalog",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganizationPath",
                schema: "catalog",
                table: "Categories",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ActorUserId",
                schema: "audit",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientType",
                schema: "audit",
                table: "AuditLogs",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                schema: "audit",
                table: "AuditLogs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImpersonated",
                schema: "audit",
                table: "AuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "audit",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "identity",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "tenancy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DefaultCulture = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Organizations_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "tenancy",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                schema: "tenancy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationPath = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InvitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "tenancy",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Memberships_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembershipRoles",
                schema: "tenancy",
                columns: table => new
                {
                    MembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipRoles", x => new { x.MembershipId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_MembershipRoles_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalSchema: "tenancy",
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembershipRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionOverrides",
                schema: "tenancy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Effect = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionOverrides_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalSchema: "tenancy",
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionOverrides_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "identity",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrganizationId",
                schema: "identity",
                table: "Roles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrganizationId_Name",
                schema: "identity",
                table: "Roles",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "[OrganizationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "identity",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FamilyId",
                schema: "identity",
                table: "RefreshTokens",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_OrganizationId",
                schema: "identity",
                table: "RefreshTokens",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationId_Sku",
                schema: "catalog",
                table: "Products",
                columns: new[] { "OrganizationId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationPath",
                schema: "catalog",
                table: "Products",
                column: "OrganizationPath");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module",
                schema: "identity",
                table: "Permissions",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_OrganizationId",
                schema: "identity",
                table: "LoginAttempts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OrganizationPath",
                schema: "catalog",
                table: "Categories",
                column: "OrganizationPath");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OrganizationId",
                schema: "audit",
                table: "AuditLogs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRoles_RoleId",
                schema: "tenancy",
                table: "MembershipRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_OrganizationId",
                schema: "tenancy",
                table: "Memberships",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserId_IsPrimary",
                schema: "tenancy",
                table: "Memberships",
                columns: new[] { "UserId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserId_OrganizationId",
                schema: "tenancy",
                table: "Memberships",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentId",
                schema: "tenancy",
                table: "Organizations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Path",
                schema: "tenancy",
                table: "Organizations",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Slug",
                schema: "tenancy",
                table: "Organizations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Type_Status",
                schema: "tenancy",
                table: "Organizations",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionOverrides_MembershipId_PermissionId",
                schema: "tenancy",
                table: "PermissionOverrides",
                columns: new[] { "MembershipId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionOverrides_PermissionId",
                schema: "tenancy",
                table: "PermissionOverrides",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                schema: "identity",
                table: "RolePermissions",
                column: "PermissionId",
                principalSchema: "identity",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("""
                UPDATE [identity].[Roles]
                SET [IsSystemRole] = 1,
                    [AllowedClients] = 1,
                    [Name] = N'PlatformAdmin',
                    [Description] = N'Ranna platform administrator'
                WHERE [Id] = '33333333-3333-3333-3333-333333333301';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "MembershipRoles",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "PermissionOverrides",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "Memberships",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "tenancy");

            migrationBuilder.DropIndex(
                name: "IX_Roles_OrganizationId",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_OrganizationId_Name",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_FamilyId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_OrganizationId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganizationId_Sku",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganizationPath",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Module",
                schema: "identity",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempts_OrganizationId",
                schema: "identity",
                table: "LoginAttempts");

            migrationBuilder.DropIndex(
                name: "IX_Categories_OrganizationPath",
                schema: "catalog",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_OrganizationId",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AllowedClients",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsSystemRole",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "OrganizationPath",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ClientType",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedByIp",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsImpersonating",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedReason",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrganizationPath",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPlatformOnly",
                schema: "identity",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "MaxScope",
                schema: "identity",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Module",
                schema: "identity",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ClientType",
                schema: "identity",
                table: "LoginAttempts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "identity",
                table: "LoginAttempts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OrganizationPath",
                schema: "catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ActorUserId",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ClientType",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsImpersonated",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "audit",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                schema: "identity",
                table: "RolePermissions",
                newName: "PermissionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "identity",
                table: "RolePermissions",
                columns: new[] { "PermissionsId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "identity",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "identity",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                schema: "catalog",
                table: "Products",
                column: "Sku",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionsId",
                schema: "identity",
                table: "RolePermissions",
                column: "PermissionsId",
                principalSchema: "identity",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
