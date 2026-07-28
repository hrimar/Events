using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsAllAndVenuesIndexSeoMetaSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PageSeoMetas",
                columns: new[] { "Id", "DescriptionBg", "DescriptionEn", "PageKey", "TitleBg", "TitleEn", "UpdatedAt" },
                values: new object[,]
                {
                    { 13, null, null, "events-all", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, null, null, "venues-index", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
