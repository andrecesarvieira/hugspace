using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class AddCorporateFeedSystem : Migration
    {

        // Campos static readonly para resolver CA1861
        private static readonly bool[] DescendingBool_falsetrue = { false, true };
        private static readonly bool[] DescendingBool_falsetruetrue = { false, true, true };
        private static readonly bool[] DescendingBool_falsefalsetrue = { false, false, true };



        // Campos static readonly para resolver CA1861
        private static readonly string[] CreatedAtIsReadArray = { "CreatedAt", "IsRead" };
        private static readonly string[] UserIdCreatedAtArray = { "UserId", "CreatedAt" };
        private static readonly string[] UserIdPriorityRelevanceScoreArray = { "UserId", "Priority", "RelevanceScore" };
        private static readonly string[] LastInteractionAtScoreArray = { "LastInteractionAt", "Score" };
        private static readonly string[] TypeInterestValueScoreArray = { "Type", "InterestValue", "Score" };
        private static readonly string[] UserIdScoreArray = { "UserId", "Score" };
        private static readonly string[] UserIdTypeInterestValueArray = { "UserId", "Type", "InterestValue" };


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endorsements_Comments_CommentId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.DropForeignKey(
                name: "FK_Endorsements_Posts_PostId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.DropIndex(
                name: "IX_Endorsements_CommentId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.DropIndex(
                name: "IX_Endorsements_PostId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.DropColumn(
                name: "CommentId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.DropColumn(
                name: "PostId1",
                schema: "Communication",
                table: "Endorsements");

            migrationBuilder.CreateTable(
                name: "FeedEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    RelevanceScore = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false, defaultValue: 0.5),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsBookmarked = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedEntries_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FeedEntries_Employees_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedEntries_Employees_UserId",
                        column: x => x.UserId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedEntries_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Communication",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedEntries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InterestValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Score = table.Column<double>(type: "double precision", precision: 4, scale: 2, nullable: false, defaultValue: 1.0),
                    InteractionCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    LastInteractionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    IsExplicit = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterests_Employees_UserId",
                        column: x => x.UserId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_AuthorId",
                table: "FeedEntries",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_CreatedAt_IsRead",
                table: "FeedEntries",
                columns: CreatedAtIsReadArray);

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_DepartmentId",
                table: "FeedEntries",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_PostId",
                table: "FeedEntries",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_TeamId",
                table: "FeedEntries",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_UserId_CreatedAt",
                table: "FeedEntries",
                columns: UserIdCreatedAtArray,
                descending: DescendingBool_falsetrue);

            migrationBuilder.CreateIndex(
                name: "IX_FeedEntries_UserId_Priority_Relevance",
                table: "FeedEntries",
                columns: UserIdPriorityRelevanceScoreArray,
                descending: DescendingBool_falsetruetrue);

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_LastInteraction_Score",
                table: "UserInterests",
                columns: LastInteractionAtScoreArray);

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_Type_Value_Score",
                table: "UserInterests",
                columns: TypeInterestValueScoreArray,
                descending: DescendingBool_falsefalsetrue);

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_UserId_Score",
                table: "UserInterests",
                columns: UserIdScoreArray,
                descending: DescendingBool_falsetrue);

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_UserId_Type_Value_Unique",
                table: "UserInterests",
                columns: UserIdTypeInterestValueArray,
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedEntries");

            migrationBuilder.DropTable(
                name: "UserInterests");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId1",
                schema: "Communication",
                table: "Endorsements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId1",
                schema: "Communication",
                table: "Endorsements",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_CommentId1",
                schema: "Communication",
                table: "Endorsements",
                column: "CommentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_PostId1",
                schema: "Communication",
                table: "Endorsements",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Endorsements_Comments_CommentId1",
                schema: "Communication",
                table: "Endorsements",
                column: "CommentId1",
                principalSchema: "Communication",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Endorsements_Posts_PostId1",
                schema: "Communication",
                table: "Endorsements",
                column: "PostId1",
                principalSchema: "Communication",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
