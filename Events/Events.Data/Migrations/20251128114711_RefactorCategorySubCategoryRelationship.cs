using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCategorySubCategoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_SubCategories_SubCategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_ParentCategory",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_ParentCategory_EnumValue",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "SubCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2733));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2735));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2737));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2739));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2741));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2742));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2744));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2745));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2747));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2749));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2991) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2994) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2996) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2997) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3031) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3039) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3040) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3042) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3044) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3045) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3049) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3053) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3054) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3057) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3064) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3066) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3069) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3070) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3072) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3073) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3079) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3082) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3085) });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CategoryId", "CreatedAt" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3088) });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId_EnumValue",
                table: "SubCategories",
                columns: new[] { "CategoryId", "EnumValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryType",
                table: "Categories",
                column: "CategoryType",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_CategoryId_EnumValue",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "SubCategories");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4898));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4901));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4903));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4904));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4905));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4906));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4907));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4909));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(4910));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5087));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5091));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5093));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5095));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5142));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5147));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5150));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5151));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5153));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5156));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5157));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5160));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5163));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5165));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5168));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5176));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5178));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5181));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5183));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5185));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5186));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5193));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5196));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5199));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 9, 17, 50, 34, 183, DateTimeKind.Utc).AddTicks(5202));

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_ParentCategory",
                table: "SubCategories",
                column: "ParentCategory");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_ParentCategory_EnumValue",
                table: "SubCategories",
                columns: new[] { "ParentCategory", "EnumValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_SubCategoryId",
                table: "Events",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryType",
                table: "Categories",
                column: "CategoryType");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_SubCategories_SubCategoryId",
                table: "Events",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
