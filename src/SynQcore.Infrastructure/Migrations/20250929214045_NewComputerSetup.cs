using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewComputerSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Departments_TargetDepartmentId",
                table: "CorporateNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Employees_ApprovedByEmployeeId",
                table: "CorporateNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Employees_CreatedByEmployeeId",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_Category",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_Category_IsActive",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_Code_Unique",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_DefaultType",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_IsActive",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_Channel",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_EmployeeId_Status",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_NextAttemptAt",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_NotificationId_EmployeeId_Unique",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_Status",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDeliveries_Status_NextAttemptAt",
                table: "NotificationDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_ExpiresAt",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_Priority",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_ScheduledFor",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_Status",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_Status_ScheduledFor",
                table: "CorporateNotifications");

            migrationBuilder.DropIndex(
                name: "IX_CorporateNotifications_Type",
                table: "CorporateNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "TitleTemplate",
                table: "NotificationTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailTemplate",
                table: "NotificationTemplates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(8000)",
                oldMaxLength: 8000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "DefaultRequiresApproval",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "DefaultRequiresAcknowledgment",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ContentTemplate",
                table: "NotificationTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NotificationTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "NotificationTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AvailablePlaceholders",
                table: "NotificationTemplates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "NotificationDeliveries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryAttempts",
                table: "NotificationDeliveries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelData",
                table: "NotificationDeliveries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CorporateNotifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresApproval",
                table: "CorporateNotifications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresAcknowledgment",
                table: "CorporateNotifications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "CorporateNotifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "CorporateNotifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Departments_TargetDepartmentId",
                table: "CorporateNotifications",
                column: "TargetDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Employees_ApprovedByEmployeeId",
                table: "CorporateNotifications",
                column: "ApprovedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Employees_CreatedByEmployeeId",
                table: "CorporateNotifications",
                column: "CreatedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Departments_TargetDepartmentId",
                table: "CorporateNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Employees_ApprovedByEmployeeId",
                table: "CorporateNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CorporateNotifications_Employees_CreatedByEmployeeId",
                table: "CorporateNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "TitleTemplate",
                table: "NotificationTemplates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationTemplates",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "EmailTemplate",
                table: "NotificationTemplates",
                type: "character varying(8000)",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "DefaultRequiresApproval",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "DefaultRequiresAcknowledgment",
                table: "NotificationTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ContentTemplate",
                table: "NotificationTemplates",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NotificationTemplates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "NotificationTemplates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AvailablePlaceholders",
                table: "NotificationTemplates",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "NotificationDeliveries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryAttempts",
                table: "NotificationDeliveries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelData",
                table: "NotificationDeliveries",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CorporateNotifications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresApproval",
                table: "CorporateNotifications",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresAcknowledgment",
                table: "CorporateNotifications",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "CorporateNotifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "CorporateNotifications",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Category",
                table: "NotificationTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Category_IsActive",
                table: "NotificationTemplates",
                columns: new[] { "Category", "IsActive" });

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

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Channel",
                table: "NotificationDeliveries",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_EmployeeId_Status",
                table: "NotificationDeliveries",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NextAttemptAt",
                table: "NotificationDeliveries",
                column: "NextAttemptAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId_EmployeeId_Unique",
                table: "NotificationDeliveries",
                columns: new[] { "NotificationId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Status",
                table: "NotificationDeliveries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_Status_NextAttemptAt",
                table: "NotificationDeliveries",
                columns: new[] { "Status", "NextAttemptAt" });

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
                columns: new[] { "Status", "ScheduledFor" });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateNotifications_Type",
                table: "CorporateNotifications",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Departments_TargetDepartmentId",
                table: "CorporateNotifications",
                column: "TargetDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Employees_ApprovedByEmployeeId",
                table: "CorporateNotifications",
                column: "ApprovedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateNotifications_Employees_CreatedByEmployeeId",
                table: "CorporateNotifications",
                column: "CreatedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
