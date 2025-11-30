using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFeaturedToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5839));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5847));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5848));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5849));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5851));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5852));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5853));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5854));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5855));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5856));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(5857));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6100));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6115));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6117));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6118));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6120));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6152));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6211));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6214));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6215));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6218));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6219));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6220));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6223));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6225));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6227));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6229));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6235));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6238));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6239));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6240));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6242));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6243));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6244));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6245));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6246));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6248));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6249));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6250));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6252));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6253));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6259));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6262));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6263));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6266));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6267));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6269));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6270));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6271));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6272));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6273));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6274));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6276));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6277));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6278));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6279));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6286));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6289));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6290));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6291));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6294));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6295));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6300));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6303));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6306));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6340));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6345));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6347));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6348));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6349));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6351));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6352));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6353));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6354));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6357));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6358));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6359));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6366));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6369));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6370));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6371));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6373));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6374));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6375));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6377));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6378));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6379));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6380));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6384));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6386));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6387));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6389));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6390));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6391));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6392));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6393));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6395));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6396));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6397));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6398));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6399));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6401));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6405));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6407));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6409));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6410));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6411));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6412));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6413));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6415));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6416));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6417));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6418));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6423));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6425));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6426));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6427));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6459));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6462));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6463));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6464));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6465));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6466));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6467));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6473));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6476));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6477));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6478));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6479));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6480));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6482));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6484));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6485));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6486));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6487));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6489));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6491));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(520));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(526));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(528));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(530));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(531));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(532));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(533));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(535));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(536));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(537));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(539));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(822));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(834));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(836));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(837));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(839));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(872));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(875));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(877));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(879));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(881));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(882));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(884));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(887));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(890));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(891));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(894));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(900));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(904));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(906));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(907));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(908));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(910));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(911));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(912));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(914));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(953));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(955));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(957));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(958));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(959));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(966));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(969));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(970));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(972));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(974));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(975));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(977));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(978));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(979));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(981));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(982));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(983));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(984));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(986));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(988));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(996));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(999));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1000));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1001));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1003));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1004));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1005));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1012));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1015));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1017));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1020));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1023));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1026));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1027));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1029));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1030));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1031));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1033));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1034));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1036));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1038));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1039));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1045));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1047));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1048));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1050));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1084));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1086));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1087));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1088));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1090));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1091));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1093));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1097));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1099));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1101));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1103));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1104));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1105));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1107));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1108));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1109));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1111));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1112));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1113));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1115));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1121));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1123));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1125));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1126));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1127));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1129));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1130));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1131));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1132));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1134));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1135));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1140));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1143));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1144));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1145));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1147));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1148));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1149));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1150));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1152));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1153));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1154));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1155));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1161));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1164));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1165));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1166));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1168));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1169));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1170));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1172));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1173));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1174));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1175));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1177));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1178));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 43, 33, 465, DateTimeKind.Utc).AddTicks(1256));
        }
    }
}
