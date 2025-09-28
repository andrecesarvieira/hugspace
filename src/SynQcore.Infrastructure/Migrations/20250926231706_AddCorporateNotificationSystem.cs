using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class AddCorporateNotificationSystem : Migration
    {

        // Campos static readonly para resolver CA1861
        private static readonly string[] StatusScheduledForArray = { "Status", "ScheduledFor" };
        private static readonly string[] EmployeeIdStatusArray = { "EmployeeId", "Status" };
        private static readonly string[] NotificationIdEmployeeIdArray = { "NotificationId", "EmployeeId" };
        private static readonly string[] StatusNextAttemptAtArray = { "Status", "NextAttemptAt" };
        private static readonly string[] CategoryIsActiveArray = { "Category", "IsActive" };


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentMentions_Comments_CommentId1",
                schema: "Communication",
                table: "CommentMentions");

            migrationBuilder.RenameColumn(
                name: "CommentId1",
                schema: "Communication",
                table: "CommentMentions",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentMentions_CommentId1",
                schema: "Communication",
                table: "CommentMentions",
                newName: "IX_CommentMentions_EmployeeId");

            migrationBuilder.CreateTable(
                name: "CorporateNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetDepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ScheduledFor = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ApprovedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequiresAcknowledgment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EnabledChannels = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateNotifications_Departments_TargetDepartmentId",
                        column: x => x.TargetDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorporateNotifications_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorporateNotifications_Employees_CreatedByEmployeeId",
                        column: x => x.CreatedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TitleTemplate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContentTemplate = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    EmailTemplate = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    DefaultType = table.Column<int>(type: "integer", nullable: false),
                    DefaultPriority = table.Column<int>(type: "integer", nullable: false),
                    DefaultChannels = table.Column<int>(type: "integer", nullable: false),
                    DefaultRequiresApproval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DefaultRequiresAcknowledgment = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AvailablePlaceholders = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReadAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveryAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ErrorDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChannelData = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_CorporateNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "CorporateNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_ApprovedByEmployeeId",
                table: "CorporateNotifications",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_CreatedByEmployeeId",
                table: "CorporateNotifications",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_ExpiresAt",
                table: "CorporateNotifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_Priority",
                table: "CorporateNotifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_ScheduledFor",
                table: "CorporateNotifications",
                column: "ScheduledFor");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_Status",
                table: "CorporateNotifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_Status_ScheduledFor",
                table: "CorporateNotifications",
                columns: StatusScheduledForArray);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_TargetDepartmentId",
                table: "CorporateNotifications",
                column: "TargetDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_Type",
                table: "CorporateNotifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Channel",
                table: "NotificationDeliveries",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_EmployeeId",
                table: "NotificationDeliveries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_EmployeeId_Status",
                table: "NotificationDeliveries",
                columns: EmployeeIdStatusArray);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NextAttemptAt",
                table: "NotificationDeliveries",
                column: "NextAttemptAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId",
                table: "NotificationDeliveries",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId_EmployeeId_Unique",
                table: "NotificationDeliveries",
                columns: NotificationIdEmployeeIdArray,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Status",
                table: "NotificationDeliveries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Status_NextAttemptAt",
                table: "NotificationDeliveries",
                columns: StatusNextAttemptAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Category",
                table: "NotificationTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Category_IsActive",
                table: "NotificationTemplates",
                columns: CategoryIsActiveArray);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Code_Unique",
                table: "NotificationTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_DefaultType",
                table: "NotificationTemplates",
                column: "DefaultType");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_IsActive",
                table: "NotificationTemplates",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentMentions_Employees_EmployeeId",
                schema: "Communication",
                table: "CommentMentions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentMentions_Employees_EmployeeId",
                schema: "Communication",
                table: "CommentMentions");

            migrationBuilder.DropTable(
                name: "NotificationDeliveries");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "CorporateNotifications");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "Communication",
                table: "CommentMentions",
                newName: "CommentId1");

            migrationBuilder.RenameIndex(
                name: "IX_CommentMentions_EmployeeId",
                schema: "Communication",
                table: "CommentMentions",
                newName: "IX_CommentMentions_CommentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentMentions_Comments_CommentId1",
                schema: "Communication",
                table: "CommentMentions",
                column: "CommentId1",
                principalSchema: "Communication",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
