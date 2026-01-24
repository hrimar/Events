using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRegisteredAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9119));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9127));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9129));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9130));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9131));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9133));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9134));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9135));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9136));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9138));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9139));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9369));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9383));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9385));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9386));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9387));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9390));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9392));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9393));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9394));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9397));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9398));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9399));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9401));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9402));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9403));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9405));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9406));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9408));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9410));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9411));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9413));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9414));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9415));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9416));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9418));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9419));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9421));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9426));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9433));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9435));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9436));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9437));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9439));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9466));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9468));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9470));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9471));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9473));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9474));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9475));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9476));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9481));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9484));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9486));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9487));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9488));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9491));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9493));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9494));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9495));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9496));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9497));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9499));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9500));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9501));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9503));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9512));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9513));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9514));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9515));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9517));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9518));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9522));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9523));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9524));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9526));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9527));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9529));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9530));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9531));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9532));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9533));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9534));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9536));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9537));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9538));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9539));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9541));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9542));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9608));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9612));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9615));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9616));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9617));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9618));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9619));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9621));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9622));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9623));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9624));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9632));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9633));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9635));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9636));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9637));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9638));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9640));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9641));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9642));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9644));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9648));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9651));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9652));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9654));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9655));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9656));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9658));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9659));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9660));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9663));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9665));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9666));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9671));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9674));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9675));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9677));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9678));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9679));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9680));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9681));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9682));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9731));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9733));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9738));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9740));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 134,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9742));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 135,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9743));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 136,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9744));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 137,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9745));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 138,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9747));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 139,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9749));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 140,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9750));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 141,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9751));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 142,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9753));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 143,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9754));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 144,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9760));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 145,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9763));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 146,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9764));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 147,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9765));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 148,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9767));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 149,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9768));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 150,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9769));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 151,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9770));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 152,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9771));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 153,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9773));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 154,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9774));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 155,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 156,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9776));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 157,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 23, 15, 14, 59, 454, DateTimeKind.Utc).AddTicks(9778));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "AspNetUsers");

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
    }
}
