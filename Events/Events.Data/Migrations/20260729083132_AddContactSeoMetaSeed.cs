using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContactSeoMetaSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PageSeoMetas",
                columns: new[] { "Id", "DescriptionBg", "DescriptionEn", "PageKey", "TitleBg", "TitleEn", "UpdatedAt" },
                values: new object[] { 15, null, null, "contact", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
