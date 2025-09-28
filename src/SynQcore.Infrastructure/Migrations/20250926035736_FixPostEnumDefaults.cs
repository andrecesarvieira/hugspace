using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SynQcore.Infrastructure.Migrations
{
    public partial class FixPostEnumDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "Communication",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }
    }
}
