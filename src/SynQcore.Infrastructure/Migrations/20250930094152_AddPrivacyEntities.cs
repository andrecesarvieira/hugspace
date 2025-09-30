using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivacyEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsentRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentCategory = table.Column<string>(type: "text", nullable: false),
                    ProcessingPurpose = table.Column<string>(type: "text", nullable: false),
                    ConsentGranted = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TermsVersion = table.Column<string>(type: "text", nullable: false),
                    CollectionEvidence = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataDeletionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletionType = table.Column<int>(type: "integer", nullable: false),
                    DataCategories = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    LegalJustification = table.Column<string>(type: "text", nullable: true),
                    IncludeBackups = table.Column<bool>(type: "boolean", nullable: false),
                    CompleteDeletion = table.Column<bool>(type: "boolean", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "integer", nullable: false),
                    EffectiveDeletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletionReport = table.Column<string>(type: "text", nullable: true),
                    VerificationHash = table.Column<string>(type: "text", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "text", nullable: true),
                    ProcessedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    NotificationSent = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataDeletionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataDeletionRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataDeletionRequests_Employees_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataExportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCategories = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    FileHash = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    DownloadExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "text", nullable: true),
                    ProcessedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    DownloadAttempts = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExportRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataExportRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataExportRequests_Employees_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonalDataCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SensitivityLevel = table.Column<int>(type: "integer", nullable: false),
                    ProcessingPurpose = table.Column<string>(type: "text", nullable: false),
                    LegalBasis = table.Column<int>(type: "integer", nullable: false),
                    RetentionPeriodMonths = table.Column<int>(type: "integer", nullable: false),
                    RequiresConsent = table.Column<bool>(type: "boolean", nullable: false),
                    IsMandatoryData = table.Column<bool>(type: "boolean", nullable: false),
                    AllowsSharing = table.Column<bool>(type: "boolean", nullable: false),
                    InternationalTransfer = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorizedCountries = table.Column<string>(type: "text", nullable: true),
                    SecurityMeasures = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IncludedFields = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDataCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_EmployeeId",
                table: "ConsentRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataDeletionRequests_EmployeeId",
                table: "DataDeletionRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataDeletionRequests_ProcessedById",
                table: "DataDeletionRequests",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_EmployeeId",
                table: "DataExportRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequests_ProcessedById",
                table: "DataExportRequests",
                column: "ProcessedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsentRecords");

            migrationBuilder.DropTable(
                name: "DataDeletionRequests");

            migrationBuilder.DropTable(
                name: "DataExportRequests");

            migrationBuilder.DropTable(
                name: "PersonalDataCategories");
        }
    }
}
