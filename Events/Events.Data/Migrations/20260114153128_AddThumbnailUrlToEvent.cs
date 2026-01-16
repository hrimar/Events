using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailUrlToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1352));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1358));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1360));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1361));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1363));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1364));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1365));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1367));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1368));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1419));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1690));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1704));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1706));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1708));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1709));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1712));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1713));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1715));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1716));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1719));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1720));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1721));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1722));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1724));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1725));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1726));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1727));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1730));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1731));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1733));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1734));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1735));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1737));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1738));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1739));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1740));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1742));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1748));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1751));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1753));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1754));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1755));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1757));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1759));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1761));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1763));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1764));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1765));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1767));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1768));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1769));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1776));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1778));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1780));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1781));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1782));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1784));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1785));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1787));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1827));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1829));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1831));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1833));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1835));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1837));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1838));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1845));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1848));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1849));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1851));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1852));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1854));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1855));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1857));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1860));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1862));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1863));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1865));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1866));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1868));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1870));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1871));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1873));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1874));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1876));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1877));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1879));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1880));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1882));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1884));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1885));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1887));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1893));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1896));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1897));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1899));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1900));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1902));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1903));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1905));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1906));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1908));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1909));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1911));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1918));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1921));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1922));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1924));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1925));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1927));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1929));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1930));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1967));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1969));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1970));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1977));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1980));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1982));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1983));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1984));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1986));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1988));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1989));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1991));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1992));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1994));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1995));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1997));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(1999));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2004));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2007));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2008));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2009));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2011));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2012));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2014));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2015));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2017));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2020));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2021));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2028));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2031));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 134,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2033));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 135,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2034));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 136,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2036));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 137,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2038));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 138,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2039));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 139,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2041));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 140,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2042));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 141,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2044));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 142,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2045));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 143,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2047));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 144,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2053));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 145,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2055));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 146,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2057));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 147,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2058));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 148,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2060));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 149,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2097));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 150,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2099));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 151,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2101));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 152,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2102));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 153,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2104));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 154,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2106));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 155,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2107));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 156,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2109));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 157,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 14, 15, 31, 26, 246, DateTimeKind.Utc).AddTicks(2110));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3805));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3814));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3815));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3817));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3818));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3819));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3821));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3822));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3823));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3824));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(3826));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4048));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4060));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4062));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4063));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4065));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4067));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4069));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4070));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4071));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4073));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4074));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4149));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4151));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4152));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4153));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4154));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4155));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4157));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4159));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4160));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4161));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4162));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4163));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4164));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4165));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4167));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4168));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4174));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4178));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4179));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4180));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4181));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4183));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4185));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4186));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4187));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4188));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4190));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4191));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4193));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4198));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4200));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4201));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4202));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4203));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4205));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4206));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4207));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4208));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4209));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4210));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4211));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4212));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4214));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4215));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4216));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4221));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4224));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4226));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4227));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4228));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4229));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4230));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4290));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4292));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4293));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4294));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4295));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4297));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4298));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4299));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4300));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4301));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4303));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4304));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4305));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4306));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4307));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4308));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4310));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4311));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4316));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4320));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4321));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4322));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4323));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4324));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4325));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4326));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4328));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4329));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4331));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4336));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4343));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4344));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4345));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4346));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4348));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4349));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4350));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4351));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4352));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4353));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4363));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4365));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4366));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4368));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4369));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4370));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4371));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4372));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4374));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4375));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4376));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4377));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4379));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4380));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4434));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4440));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4442));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4443));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4444));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4445));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4446));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4447));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4448));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4452));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4453));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4458));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4461));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 134,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4462));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 135,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4464));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 136,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4465));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 137,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4466));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 138,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4467));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 139,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4468));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 140,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4469));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 141,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4471));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 142,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4472));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 143,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4473));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 144,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4477));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 145,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4480));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 146,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4482));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 147,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4483));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 148,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4484));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 149,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4485));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 150,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4486));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 151,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4487));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 152,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4489));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 153,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4490));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 154,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4491));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 155,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4492));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 156,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4493));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 157,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 58, 55, 869, DateTimeKind.Utc).AddTicks(4495));
        }
    }
}
