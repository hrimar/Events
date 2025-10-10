using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentCategory = table.Column<int>(type: "int", nullable: false),
                    EnumValue = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2842));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2846));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2848));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2850));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2852));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2854));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2855));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2857));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2859));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3228), "Rock music events and concerts", 1, "Rock", 1 },
                    { 2, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3233), "Jazz performances and sessions", 2, "Jazz", 1 },
                    { 3, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3236), "Heavy metal and metal subgenres", 3, "Metal", 1 },
                    { 4, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3238), "Pop music events", 4, "Pop", 1 },
                    { 5, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3279), "Funk music events", 5, "Funk", 1 },
                    { 6, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3285), "Punk music events", 6, "Punk", 1 },
                    { 7, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3290), "Opera music events", 7, "Opera", 1 },
                    { 8, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3292), "Classical music concerts", 8, "Classical", 1 },
                    { 9, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3295), "Electronic music and DJ sets", 9, "Electronic", 1 },
                    { 10, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3299), "Folk and traditional music", 10, "Folk", 1 },
                    { 11, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3301), "Blues music performances", 11, "Blues", 1 },
                    { 12, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3305), "Country music events", 12, "Country", 1 },
                    { 13, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3311), "Reggae music events", 13, "Reggae", 1 },
                    { 14, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3313), "Hip-hop and rap events", 14, "HipHop", 1 },
                    { 15, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3317), "Alternative music events", 15, "Alternative", 1 },
                    { 16, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3326), "Football matches and events", 1, "Football", 4 },
                    { 17, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3387), "Basketball games and tournaments", 2, "Basketball", 4 },
                    { 18, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3392), "Tennis matches and tournaments", 3, "Tennis", 4 },
                    { 19, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3394), "Volleyball games", 4, "Volleyball", 4 },
                    { 20, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3397), "Swimming competitions", 5, "Swimming", 4 },
                    { 21, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3399), "Track and field events", 6, "Athletics", 4 },
                    { 22, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3407), "Boxing events and competitions", 7, "Boxing", 4 },
                    { 23, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3412), "Wrestling events and competitions", 8, "Wrestling", 4 },
                    { 24, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3417), "Gymnastics events and competitions", 9, "Gymnastics", 4 },
                    { 25, new DateTime(2025, 10, 9, 15, 43, 29, 641, DateTimeKind.Utc).AddTicks(3421), "Cycling events and competitions", 10, "Cycling", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_ParentCategory",
                table: "SubCategories",
                column: "ParentCategory");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_ParentCategory_EnumValue",
                table: "SubCategories",
                columns: new[] { "ParentCategory", "EnumValue" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_SubCategories_SubCategoryId",
                table: "Events",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_SubCategories_SubCategoryId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8842));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8848));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8849));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8851));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8852));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8853));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8854));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8856));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8856));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 18, 19, 19, 909, DateTimeKind.Utc).AddTicks(8857));
        }
    }
}
