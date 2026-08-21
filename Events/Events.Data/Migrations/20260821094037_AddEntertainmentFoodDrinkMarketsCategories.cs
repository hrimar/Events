using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntertainmentFoodDrinkMarketsCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CategoryType",
                value: 14);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryType", "CreatedAt", "DefaultImageUrl", "Description", "Name" },
                values: new object[,]
                {
                    { 12, 11, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Parties, quizzes, games and social entertainment", "Entertainment" },
                    { 13, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tastings, culinary dinners and food events", "FoodDrink" },
                    { 14, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Craft, farmers and flea markets", "Markets" }
                });

            migrationBuilder.InsertData(
                table: "PageSeoMetas",
                columns: new[] { "Id", "DescriptionBg", "DescriptionEn", "PageKey", "TitleBg", "TitleEn", "UpdatedAt" },
                values: new object[,]
                {
                    { 16, null, null, "category-entertainment", null, null, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, null, null, "category-fooddrink", null, null, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, null, null, "category-markets", null, null, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[,]
                {
                    { 158, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "General parties with no announced artist", 1, "Party", 11 },
                    { 159, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Themed parties (e.g. Gatsby Night, Retro Party)", 2, "ThemedParty", 11 },
                    { 160, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Club nights and DJ sets with no headline artist", 3, "ClubNight", 11 },
                    { 161, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Pub quiz and trivia nights", 4, "Quiz", 11 },
                    { 162, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Karaoke nights", 5, "Karaoke", 11 },
                    { 163, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Board game nights", 6, "BoardGames", 11 },
                    { 164, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Escape rooms and quests", 7, "EscapeRoom", 11 },
                    { 165, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Social dance parties (salsa, bachata, swing)", 8, "SocialDance", 11 },
                    { 166, 12, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Other entertainment events", 99, "Other", 11 },
                    { 167, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Wine tasting events", 1, "WineTasting", 12 },
                    { 168, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Craft beer tasting events", 2, "BeerTasting", 12 },
                    { 169, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Spirits tasting events", 3, "SpiritsTasting", 12 },
                    { 170, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Chef dinners and special menus", 4, "CulinaryDinner", 12 },
                    { 171, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Brunch events", 5, "Brunch", 12 },
                    { 172, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Street food events", 6, "StreetFood", 12 },
                    { 173, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Coffee and tea events", 7, "CoffeeTea", 12 },
                    { 174, 13, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Other food and drink events", 99, "Other", 12 },
                    { 175, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Handmade craft markets", 1, "CraftMarket", 13 },
                    { 176, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Farmers markets", 2, "FarmersMarket", 13 },
                    { 177, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Flea and vintage markets", 3, "FleaMarket", 13 },
                    { 178, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Christmas markets", 4, "ChristmasMarket", 13 },
                    { 179, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Design markets", 5, "DesignMarket", 13 },
                    { 180, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Book fairs and markets", 6, "BookMarket", 13 },
                    { 181, 14, new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Other markets", 99, "Other", 13 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "PageSeoMetas",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CategoryType",
                value: 11);
        }
    }
}
