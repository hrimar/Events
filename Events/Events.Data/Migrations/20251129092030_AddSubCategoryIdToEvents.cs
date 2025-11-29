using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoryIdToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4965));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4972));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4974));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4976));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4977));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4979));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4980));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4982));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4983));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4985));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(4986));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5268));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5283));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5284));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5286));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5288));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5330));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5334));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5337));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5339));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5341));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5343));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5345));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5348));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5352));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5353));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5357));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5366));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5371));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5372));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5374));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5375));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5377));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5379));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5381));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5382));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5384));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5386));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5388));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5389));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5391));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5398));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5401));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5403));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5486));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5488));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5490));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5491));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5493));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5494));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5496));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5498));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5499));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5501));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5502));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5504));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5506));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5512));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5517));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5518));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5520));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5521));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5523));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5524));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5534));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5538));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5541));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5545));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5550));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5553));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5555));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5556));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5559));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5561));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5562));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5566));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5568));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5569));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5575));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5578));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5579));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5581));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5582));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5584));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5585));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5587));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5755));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5760));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5762));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5807));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5811));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5813));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5814));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5816));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5817));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5819));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5820));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5822));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5824));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5826));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5827));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5829));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5831));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5838));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5842));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5843));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5845));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5846));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5848));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5850));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5852));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5854));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5855));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5857));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5863));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5866));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5867));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5869));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5870));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5872));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5874));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5875));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5876));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5878));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5880));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5881));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5888));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5892));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5893));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5895));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5897));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5899));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5901));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5903));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5905));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5906));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5908));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5909));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 9, 20, 28, 984, DateTimeKind.Utc).AddTicks(5950));

            migrationBuilder.CreateIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events",
                column: "SubCategoryId");

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

            migrationBuilder.DropIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5853));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5862));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5864));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5866));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5867));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5868));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5869));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5871));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5872));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5873));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5874));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6074));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6089));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6091));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6092));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6135));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6138));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6154));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6155));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6157));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6159));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6160));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6163));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6171));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6172));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6175));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6181));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6185));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6187));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6188));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6189));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6234));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6235));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6236));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6237));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6238));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6239));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6240));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6241));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6243));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6252));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6255));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6257));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6259));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6260));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6262));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6263));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6264));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6266));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6268));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6269));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6270));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6271));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6272));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6273));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6279));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6282));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6283));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6284));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6286));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6287));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6288));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6296));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6299));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6302));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6305));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6312));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6314));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6315));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6316));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6317));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6319));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6320));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6322));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6324));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6325));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6331));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6333));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6335));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6336));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6337));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6338));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6340));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6341));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6342));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6343));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6344));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6350));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6352));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6354));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6355));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6356));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6357));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6358));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6360));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6394));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6396));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6397));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6398));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6399));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6401));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6409));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6412));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6413));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6414));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6415));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6417));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6418));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6419));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6420));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6421));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6422));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6427));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6430));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6431));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6432));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6433));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6434));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6436));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6437));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6438));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6439));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6440));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6441));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6447));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6451));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6452));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6453));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6455));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6456));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6457));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6458));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6459));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6461));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6462));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6465));
        }
    }
}
