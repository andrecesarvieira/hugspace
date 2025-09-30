using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1861 // Arrays constantes em migrations são aceitáveis

namespace SynQcore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Moderations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentAuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModeratorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Low"),
                    ActionTaken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ModeratorNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AdditionalDetails = table.Column<string>(type: "jsonb", nullable: true),
                    IsAutomaticDetection = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AutoDetectionScore = table.Column<short>(type: "smallint", nullable: true),
                    ModerationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moderations_Employees_ContentAuthorId",
                        column: x => x.ContentAuthorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Moderations_Employees_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Moderations_Employees_ReportedByEmployeeId",
                        column: x => x.ReportedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ModerationAppeals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModerationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppealByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Evidence = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ReviewerResponse = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Decision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultAction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Normal"),
                    AdditionalData = table.Column<string>(type: "jsonb", nullable: true),
                    UserIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ResponseDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationAppeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_Employees_AppealByEmployeeId",
                        column: x => x.AppealByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_Employees_ReviewedByEmployeeId",
                        column: x => x.ReviewedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_Moderations_ModerationId",
                        column: x => x.ModerationId,
                        principalTable: "Moderations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModerationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PreviousStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AdditionalData = table.Column<string>(type: "jsonb", nullable: true),
                    UserIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAutomaticAction = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationLogs_Employees_ActionByEmployeeId",
                        column: x => x.ActionByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ModerationLogs_Moderations_ModerationId",
                        column: x => x.ModerationId,
                        principalTable: "Moderations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPunishments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModerationId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppliedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPermanent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Restrictions = table.Column<string>(type: "jsonb", nullable: true),
                    PreviousWarnings = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsRecurrence = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    InfractionPoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModeratorIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    AdditionalData = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPunishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPunishments_Employees_AppliedByEmployeeId",
                        column: x => x.AppliedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPunishments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPunishments_Moderations_ModerationId",
                        column: x => x.ModerationId,
                        principalTable: "Moderations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_AppealByEmployeeId",
                table: "ModerationAppeals",
                column: "AppealByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_CreatedAt",
                table: "ModerationAppeals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_ModerationId",
                table: "ModerationAppeals",
                column: "ModerationId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Priority",
                table: "ModerationAppeals",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_ReviewedByEmployeeId",
                table: "ModerationAppeals",
                column: "ReviewedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Status",
                table: "ModerationAppeals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Status_CreatedAt",
                table: "ModerationAppeals",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Status_Priority",
                table: "ModerationAppeals",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Type",
                table: "ModerationAppeals",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_Action",
                table: "ModerationLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_Action_CreatedAt",
                table: "ModerationLogs",
                columns: new[] { "Action", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_ActionByEmployeeId",
                table: "ModerationLogs",
                column: "ActionByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_CreatedAt",
                table: "ModerationLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_ModerationId",
                table: "ModerationLogs",
                column: "ModerationId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationLogs_ModerationId_CreatedAt",
                table: "ModerationLogs",
                columns: new[] { "ModerationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_Category",
                table: "Moderations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_Category_Severity",
                table: "Moderations",
                columns: new[] { "Category", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_ContentAuthorId",
                table: "Moderations",
                column: "ContentAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_ContentId",
                table: "Moderations",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_CreatedAt",
                table: "Moderations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_ModeratorId",
                table: "Moderations",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_ReportedByEmployeeId",
                table: "Moderations",
                column: "ReportedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_Severity",
                table: "Moderations",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_Status",
                table: "Moderations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Moderations_Status_CreatedAt",
                table: "Moderations",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_AppliedByEmployeeId",
                table: "UserPunishments",
                column: "AppliedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_EmployeeId",
                table: "UserPunishments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_EmployeeId_Status",
                table: "UserPunishments",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_EndDate",
                table: "UserPunishments",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_ModerationId",
                table: "UserPunishments",
                column: "ModerationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_Severity",
                table: "UserPunishments",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_StartDate",
                table: "UserPunishments",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_StartDate_EndDate",
                table: "UserPunishments",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_Type",
                table: "UserPunishments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_UserPunishments_Type_Status",
                table: "UserPunishments",
                columns: new[] { "Type", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModerationAppeals");

            migrationBuilder.DropTable(
                name: "ModerationLogs");

            migrationBuilder.DropTable(
                name: "UserPunishments");

            migrationBuilder.DropTable(
                name: "Moderations");
        }
    }
}
