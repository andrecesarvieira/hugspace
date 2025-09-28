using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class AddKnowledgeManagementEntities : Migration
    {

        // Campos static readonly para resolver CA1861
        private static readonly string[] TypeStatusVisibilityArray = { "Type", "Status", "Visibility" };


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Departments_DepartmentId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Employees_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Teams_TeamId",
                table: "Posts");

            migrationBuilder.EnsureSchema(
                name: "Communication");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Posts",
                newSchema: "Communication");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "TeamMemberships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "Communication",
                table: "Posts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentUrl",
                schema: "Communication",
                table: "Posts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Communication",
                table: "Posts",
                type: "character varying(50000)",
                maxLength: 50000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "Communication",
                table: "Posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentPostId",
                schema: "Communication",
                table: "Posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                schema: "Communication",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                schema: "Communication",
                table: "Posts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Communication",
                table: "Posts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                schema: "Communication",
                table: "Posts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "1.0");

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KnowledgeCategories",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#007ACC"),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "📄"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeCategories_KnowledgeCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalSchema: "Communication",
                        principalTable: "KnowledgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#6B7280"),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                schema: "Communication",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostTags_Employees_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Communication",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Communication",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                schema: "Communication",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt",
                schema: "Communication",
                table: "Posts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ParentPostId",
                schema: "Communication",
                table: "Posts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status",
                schema: "Communication",
                table: "Posts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Type",
                schema: "Communication",
                table: "Posts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Type_Status_Visibility",
                schema: "Communication",
                table: "Posts",
                columns: TypeStatusVisibilityArray);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ViewCount",
                schema: "Communication",
                table: "Posts",
                column: "ViewCount");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Visibility",
                schema: "Communication",
                table: "Posts",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCategories_Name",
                schema: "Communication",
                table: "KnowledgeCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCategories_ParentCategoryId",
                schema: "Communication",
                table: "KnowledgeCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_AddedAt",
                schema: "Communication",
                table: "PostTags",
                column: "AddedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_AddedById",
                schema: "Communication",
                table: "PostTags",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                schema: "Communication",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "Communication",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Type",
                schema: "Communication",
                table: "Tags",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UsageCount",
                schema: "Communication",
                table: "Tags",
                column: "UsageCount");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Departments_DepartmentId",
                schema: "Communication",
                table: "Posts",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Employees_AuthorId",
                schema: "Communication",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_KnowledgeCategories_CategoryId",
                schema: "Communication",
                table: "Posts",
                column: "CategoryId",
                principalSchema: "Communication",
                principalTable: "KnowledgeCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ParentPostId",
                schema: "Communication",
                table: "Posts",
                column: "ParentPostId",
                principalSchema: "Communication",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Teams_TeamId",
                schema: "Communication",
                table: "Posts",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Departments_DepartmentId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Employees_AuthorId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_KnowledgeCategories_CategoryId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ParentPostId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Teams_TeamId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "KnowledgeCategories",
                schema: "Communication");

            migrationBuilder.DropTable(
                name: "PostTags",
                schema: "Communication");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Communication");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CategoryId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedAt",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ParentPostId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Status",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Type",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Type_Status_Visibility",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ViewCount",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Visibility",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "TeamMemberships");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ParentPostId",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Summary",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                schema: "Communication",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "Communication",
                newName: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentUrl",
                table: "Posts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50000)",
                oldMaxLength: 50000);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Departments_DepartmentId",
                table: "Posts",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Employees_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Teams_TeamId",
                table: "Posts",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
