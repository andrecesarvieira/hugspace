using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class AddCorporateDocumentManagementSystem : Migration
    {

        // Campos static readonly para resolver CA1861
        private static readonly string[] StatusAccessLevelArray = { "Status", "AccessLevel" };
        private static readonly string[] TitleTypeArray = { "Title", "Type" };
        private static readonly string[] DocumentIdDepartmentIdArray = { "DocumentId", "DepartmentId" };
        private static readonly string[] DocumentIdEmployeeIdArray = { "DocumentId", "EmployeeId" };
        private static readonly string[] DocumentIdRoleArray = { "DocumentId", "Role" };
        private static readonly string[] ActionAccessedAtArray = { "Action", "AccessedAt" };
        private static readonly string[] DocumentIdEmployeeIdActionAccessedAtArray = { "DocumentId", "EmployeeId", "Action", "AccessedAt" };
        private static readonly string[] DocumentIdAccessedAtArray = { "DocumentId", "AccessedAt" };
        private static readonly string[] EmployeeIdAccessedAtArray = { "EmployeeId", "AccessedAt" };
        private static readonly string[] DocumentTypeIsActiveArray = { "DocumentType", "IsActive" };
        private static readonly string[] IsDefaultDocumentTypeArray = { "IsDefault", "DocumentType" };
        private static readonly string[] DocumentTypeIsDefaultArray = { "DocumentType", "IsDefault" };
        private static readonly string[] CategoryIsApprovedArray = { "Category", "IsApproved" };
        private static readonly string[] TypeIsApprovedArray = { "Type", "IsApproved" };


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporateDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StorageFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    UploadedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ParentDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsCurrentVersion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FileHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalStorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateDocuments_CorporateDocuments_ParentDocumentId",
                        column: x => x.ParentDocumentId,
                        principalTable: "CorporateDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorporateDocuments_Departments_OwnerDepartmentId",
                        column: x => x.OwnerDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorporateDocuments_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorporateDocuments_Employees_UploadedByEmployeeId",
                        column: x => x.UploadedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    DefaultCategory = table.Column<int>(type: "integer", nullable: false),
                    DefaultAccessLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    TemplateFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Placeholders = table.Column<string>(type: "jsonb", nullable: true),
                    UsageInstructions = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    OwnerDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTemplates_Departments_OwnerDepartmentId",
                        column: x => x.OwnerDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DocumentTemplates_Employees_CreatedByEmployeeId",
                        column: x => x.CreatedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StorageFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    StorageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UploadedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaAssets_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MediaAssets_Employees_UploadedByEmployeeId",
                        column: x => x.UploadedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccessType = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    GrantedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAccesses", x => x.Id);
                    table.CheckConstraint("CK_DocumentAccesses_OneAccessType", "((\"EmployeeId\" IS NOT NULL) AND (\"DepartmentId\" IS NULL) AND (\"Role\" IS NULL)) OR ((\"EmployeeId\" IS NULL) AND (\"DepartmentId\" IS NOT NULL) AND (\"Role\" IS NULL)) OR ((\"EmployeeId\" IS NULL) AND (\"DepartmentId\" IS NULL) AND (\"Role\" IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_DocumentAccesses_CorporateDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CorporateDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAccesses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAccesses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAccesses_Employees_GrantedByEmployeeId",
                        column: x => x.GrantedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    AccessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SessionDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAccessLogs_CorporateDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CorporateDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentAccessLogs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_AccessLevel",
                table: "CorporateDocuments",
                column: "AccessLevel");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_ApprovedByEmployeeId",
                table: "CorporateDocuments",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Category",
                table: "CorporateDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_IsCurrentVersion",
                table: "CorporateDocuments",
                column: "IsCurrentVersion");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_OwnerDepartmentId",
                table: "CorporateDocuments",
                column: "OwnerDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_ParentDocumentId",
                table: "CorporateDocuments",
                column: "ParentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Status",
                table: "CorporateDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Status_AccessLevel",
                table: "CorporateDocuments",
                columns: StatusAccessLevelArray);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Title_Search",
                table: "CorporateDocuments",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Title_Type",
                table: "CorporateDocuments",
                columns: TitleTypeArray);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_Type",
                table: "CorporateDocuments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateDocuments_UploadedByEmployeeId",
                table: "CorporateDocuments",
                column: "UploadedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_AccessType",
                table: "DocumentAccesses",
                column: "AccessType");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_DepartmentId",
                table: "DocumentAccesses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_Document_Department",
                table: "DocumentAccesses",
                columns: DocumentIdDepartmentIdArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_Document_Employee",
                table: "DocumentAccesses",
                columns: DocumentIdEmployeeIdArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_Document_Role",
                table: "DocumentAccesses",
                columns: DocumentIdRoleArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_DocumentId",
                table: "DocumentAccesses",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_EmployeeId",
                table: "DocumentAccesses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_GrantedByEmployeeId",
                table: "DocumentAccesses",
                column: "GrantedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_IsActive",
                table: "DocumentAccesses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccesses_Role",
                table: "DocumentAccesses",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_AccessedAt",
                table: "DocumentAccessLogs",
                column: "AccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_Action",
                table: "DocumentAccessLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_Action_AccessedAt",
                table: "DocumentAccessLogs",
                columns: ActionAccessedAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_Audit",
                table: "DocumentAccessLogs",
                columns: DocumentIdEmployeeIdActionAccessedAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_Document_AccessedAt",
                table: "DocumentAccessLogs",
                columns: DocumentIdAccessedAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_DocumentId",
                table: "DocumentAccessLogs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_Employee_AccessedAt",
                table: "DocumentAccessLogs",
                columns: EmployeeIdAccessedAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAccessLogs_EmployeeId",
                table: "DocumentAccessLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_CreatedByEmployeeId",
                table: "DocumentTemplates",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_DefaultCategory",
                table: "DocumentTemplates",
                column: "DefaultCategory");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_DocumentType",
                table: "DocumentTemplates",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_DocumentType_IsActive",
                table: "DocumentTemplates",
                columns: DocumentTypeIsActiveArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_IsActive",
                table: "DocumentTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_IsDefault",
                table: "DocumentTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_IsDefault_DocumentType",
                table: "DocumentTemplates",
                columns: IsDefaultDocumentTypeArray);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_Name",
                table: "DocumentTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_OwnerDepartmentId",
                table: "DocumentTemplates",
                column: "OwnerDepartmentId");

            migrationBuilder.CreateIndex(
                name: "UX_DocumentTemplates_DefaultPerType",
                table: "DocumentTemplates",
                columns: DocumentTypeIsDefaultArray,
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_AccessLevel",
                table: "MediaAssets",
                column: "AccessLevel");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_ApprovedByEmployeeId",
                table: "MediaAssets",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Category",
                table: "MediaAssets",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Category_IsApproved",
                table: "MediaAssets",
                columns: CategoryIsApprovedArray);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_IsApproved",
                table: "MediaAssets",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Name_Search",
                table: "MediaAssets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Type",
                table: "MediaAssets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Type_IsApproved",
                table: "MediaAssets",
                columns: TypeIsApprovedArray);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_UploadedByEmployeeId",
                table: "MediaAssets",
                column: "UploadedByEmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAccesses");

            migrationBuilder.DropTable(
                name: "DocumentAccessLogs");

            migrationBuilder.DropTable(
                name: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "MediaAssets");

            migrationBuilder.DropTable(
                name: "CorporateDocuments");
        }
    }
}
