using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeQuickFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeQuickFilters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabelBg = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LabelEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeQuickFilters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HomeQuickFilters",
                columns: new[] { "Id", "IsActive", "LabelBg", "LabelEn", "SortOrder", "Type", "UpdatedAt", "Values" },
                values: new object[,]
                {
                    { 1, true, "Театър", "Theatre", 1, 1, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"Theatre\"]" },
                    { 2, true, "Музика", "Music", 2, 1, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"Music\"]" },
                    { 3, true, "Кино", "Cinema", 3, 1, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"Cinema\"]" },
                    { 4, true, "Бизнес", "Business", 4, 1, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"Business\",\"Conferences\"]" },
                    { 5, true, "Уъркшопи", "Workshops", 5, 1, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"Workshops\"]" },
                    { 6, true, "За деца", "For kids", 6, 2, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[\"For kids\",\"Family-friendly\"]" },
                    { 7, true, "Безплатни", "Free", 7, 3, new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), "[]" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeQuickFilters_SortOrder",
                table: "HomeQuickFilters",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeQuickFilters");
        }
    }
}
