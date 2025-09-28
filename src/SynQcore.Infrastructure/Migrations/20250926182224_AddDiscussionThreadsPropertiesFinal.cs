using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class AddDiscussionThreadsPropertiesFinal : Migration
    {

        // Campos static readonly para resolver CA1861
        private static readonly string[] PostIdParentCommentIdCreatedAtArray = { "PostId", "ParentCommentId", "CreatedAt" };
        private static readonly string[] MentionedEmployeeIdHasBeenNotifiedArray = { "MentionedEmployeeId", "HasBeenNotified" };
        private static readonly string[] MentionedEmployeeIdIsReadArray = { "MentionedEmployeeId", "IsRead" };
        private static readonly string[] CommentIdEndorserIdArray = { "CommentId", "EndorserId" };
        private static readonly string[] PostIdEndorserIdArray = { "PostId", "EndorserId" };


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Employees_AuthorId",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comments",
                newSchema: "Communication");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                schema: "Communication",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedEntityId",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelatedEntityType",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Communication",
                table: "Comments",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "EndorsementCount",
                schema: "Communication",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfidential",
                schema: "Communication",
                table: "Comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighlighted",
                schema: "Communication",
                table: "Comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                schema: "Communication",
                table: "Comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                schema: "Communication",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                schema: "Communication",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAt",
                schema: "Communication",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModeratedById",
                schema: "Communication",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationReason",
                schema: "Communication",
                table: "Comments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationStatus",
                schema: "Communication",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                schema: "Communication",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                schema: "Communication",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNote",
                schema: "Communication",
                table: "Comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                schema: "Communication",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResolvedById",
                schema: "Communication",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThreadLevel",
                schema: "Communication",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThreadPath",
                schema: "Communication",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "Communication",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                schema: "Communication",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CommentMentions",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionedEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionedById = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionText = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartPosition = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    HasBeenNotified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Context = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Urgency = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CommentId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentMentions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "Communication",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentMentions_Comments_CommentId1",
                        column: x => x.CommentId1,
                        principalSchema: "Communication",
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentMentions_Employees_MentionedById",
                        column: x => x.MentionedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentMentions_Employees_MentionedEmployeeId",
                        column: x => x.MentionedEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Endorsements",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    EndorserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EndorsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Context = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CommentId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endorsements", x => x.Id);
                    table.CheckConstraint("CK_Endorsement_ContentType", "(\"PostId\" IS NOT NULL AND \"CommentId\" IS NULL) OR (\"PostId\" IS NULL AND \"CommentId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Endorsements_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "Communication",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Endorsements_Comments_CommentId1",
                        column: x => x.CommentId1,
                        principalSchema: "Communication",
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Endorsements_Employees_EndorserId",
                        column: x => x.EndorserId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Endorsements_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Communication",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Endorsements_Posts_PostId1",
                        column: x => x.PostId1,
                        principalSchema: "Communication",
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedAt",
                schema: "Communication",
                table: "Comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ModeratedById",
                schema: "Communication",
                table: "Comments",
                column: "ModeratedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ModerationStatus",
                schema: "Communication",
                table: "Comments",
                column: "ModerationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_ParentCommentId_CreatedAt",
                schema: "Communication",
                table: "Comments",
                columns: PostIdParentCommentIdCreatedAtArray);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ResolvedById",
                schema: "Communication",
                table: "Comments",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Type",
                schema: "Communication",
                table: "Comments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_CommentId",
                schema: "Communication",
                table: "CommentMentions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_CommentId1",
                schema: "Communication",
                table: "CommentMentions",
                column: "CommentId1");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_CreatedAt",
                schema: "Communication",
                table: "CommentMentions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_MentionedById",
                schema: "Communication",
                table: "CommentMentions",
                column: "MentionedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_MentionedEmployeeId",
                schema: "Communication",
                table: "CommentMentions",
                column: "MentionedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_MentionedEmployeeId_HasBeenNotified",
                schema: "Communication",
                table: "CommentMentions",
                columns: MentionedEmployeeIdHasBeenNotifiedArray);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_MentionedEmployeeId_IsRead",
                schema: "Communication",
                table: "CommentMentions",
                columns: MentionedEmployeeIdIsReadArray);

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_Comment_Endorser_Unique",
                schema: "Communication",
                table: "Endorsements",
                columns: CommentIdEndorserIdArray,
                unique: true,
                filter: "\"CommentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_CommentId",
                schema: "Communication",
                table: "Endorsements",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_CommentId1",
                schema: "Communication",
                table: "Endorsements",
                column: "CommentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_Context",
                schema: "Communication",
                table: "Endorsements",
                column: "Context");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_EndorsedAt",
                schema: "Communication",
                table: "Endorsements",
                column: "EndorsedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_EndorserId",
                schema: "Communication",
                table: "Endorsements",
                column: "EndorserId");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_Post_Endorser_Unique",
                schema: "Communication",
                table: "Endorsements",
                columns: PostIdEndorserIdArray,
                unique: true,
                filter: "\"PostId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_PostId",
                schema: "Communication",
                table: "Endorsements",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_PostId1",
                schema: "Communication",
                table: "Endorsements",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_Type",
                schema: "Communication",
                table: "Endorsements",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "Communication",
                table: "Comments",
                column: "ParentCommentId",
                principalSchema: "Communication",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Employees_AuthorId",
                schema: "Communication",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Employees_ModeratedById",
                schema: "Communication",
                table: "Comments",
                column: "ModeratedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Employees_ResolvedById",
                schema: "Communication",
                table: "Comments",
                column: "ResolvedById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Employees_AuthorId",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Employees_ModeratedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Employees_ResolvedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "CommentMentions",
                schema: "Communication");

            migrationBuilder.DropTable(
                name: "Endorsements",
                schema: "Communication");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedAt",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ModeratedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ModerationStatus",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostId_ParentCommentId_CreatedAt",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ResolvedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Type",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RelatedEntityType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EndorsementCount",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsConfidential",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsHighlighted",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModeratedAt",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModeratedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModerationReason",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReplyCount",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ResolutionNote",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ResolvedById",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ThreadLevel",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ThreadPath",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Visibility",
                schema: "Communication",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                schema: "Communication",
                newName: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Employees_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
