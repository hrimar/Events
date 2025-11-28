using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllSubcategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SubCategories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryType", "CreatedAt", "DefaultImageUrl", "Description", "Name" },
                values: new object[] { 11, 11, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(5874), null, "Events pending categorization", "Undefined" });

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
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6175), "Other music events", 99, "Other", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6181), "Painting exhibitions and shows", 1, "Painting", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6185), "Sculpture exhibitions", 2, "Sculpture", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6187), "Photography exhibitions", 3, "Photography", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6188), "Digital art exhibitions", 4, "DigitalArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6189), "Street art and urban art events", 5, "StreetArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6234), "Graffiti art exhibitions", 6, "Graffiti", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6235), "Illustration exhibitions", 7, "Illustration", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6236), "Performance art events", 8, "PerformanceArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6237), "Installation art exhibitions", 9, "InstallationArt", 2 });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[,]
                {
                    { 26, 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6238), "Contemporary art exhibitions", 10, "ContemporaryArt", 2 },
                    { 27, 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6239), "Visual arts events", 11, "VisualArts", 2 },
                    { 28, 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6240), "Mixed media art exhibitions", 12, "MixedMedia", 2 },
                    { 29, 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6241), "Conceptual art exhibitions", 13, "ConceptualArt", 2 },
                    { 30, 2, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6243), "Other art events", 99, "Other", 2 },
                    { 31, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6252), "Business networking events", 1, "NetworkingEvents", 3 },
                    { 32, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6255), "Startup events and pitches", 2, "Startups", 3 },
                    { 33, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6257), "Entrepreneurship seminars", 3, "Entrepreneurship", 3 },
                    { 34, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6259), "Marketing conferences", 4, "Marketing", 3 },
                    { 35, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6260), "Sales training and events", 5, "Sales", 3 },
                    { 36, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6262), "Leadership development events", 6, "Leadership", 3 },
                    { 37, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6263), "Finance and investment events", 7, "Finance", 3 },
                    { 38, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6264), "Real estate events", 8, "RealEstate", 3 },
                    { 39, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6265), "Investment seminars", 9, "Investment", 3 },
                    { 40, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6266), "E-commerce events", 10, "ECommerce", 3 },
                    { 41, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6268), "Innovation forums", 11, "Innovation", 3 },
                    { 42, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6269), "Technology business events", 12, "Technology", 3 },
                    { 43, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6270), "HR and management events", 13, "HRManagement", 3 },
                    { 44, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6271), "Business strategy workshops", 14, "BusinessStrategy", 3 },
                    { 45, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6272), "Product development events", 15, "ProductDevelopment", 3 },
                    { 46, 3, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6273), "Other business events", 99, "Other", 3 },
                    { 47, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6279), "Football matches and events", 1, "Football", 4 },
                    { 48, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6282), "Basketball games and tournaments", 2, "Basketball", 4 },
                    { 49, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6283), "Tennis matches and tournaments", 3, "Tennis", 4 },
                    { 50, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6284), "Volleyball games", 4, "Volleyball", 4 },
                    { 51, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6286), "Swimming competitions", 5, "Swimming", 4 },
                    { 52, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6287), "Track and field events", 6, "Athletics", 4 },
                    { 53, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6288), "Boxing events and competitions", 7, "Boxing", 4 },
                    { 54, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6296), "Wrestling events and competitions", 8, "Wrestling", 4 },
                    { 55, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6299), "Gymnastics events and competitions", 9, "Gymnastics", 4 },
                    { 56, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6302), "Cycling events and competitions", 10, "Cycling", 4 },
                    { 57, 4, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6305), "Other sports events and competitions", 99, "Other", 4 },
                    { 58, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6310), "Drama theatre performances", 1, "Drama", 5 },
                    { 59, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6312), "Comedy theatre shows", 2, "Comedy", 5 },
                    { 60, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6314), "Musical theatre productions", 3, "MusicalTheatre", 5 },
                    { 61, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6315), "Tragedy theatre performances", 4, "Tragedy", 5 },
                    { 62, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6316), "Experimental theatre", 5, "ExperimentalTheatre", 5 },
                    { 63, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6317), "Puppet theatre shows", 6, "PuppetTheatre", 5 },
                    { 64, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6319), "Improvisation theatre", 7, "Improvisation", 5 },
                    { 65, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6320), "Street theatre performances", 8, "StreetTheatre", 5 },
                    { 66, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6322), "Monodrama performances", 9, "Monodrama", 5 },
                    { 67, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6324), "Children's theatre shows", 10, "ChildrensTheatre", 5 },
                    { 68, 5, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6325), "Other theatre performances", 99, "Other", 5 },
                    { 69, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6331), "Feature film screenings", 1, "FeatureFilms", 6 },
                    { 70, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6333), "Short film screenings", 2, "ShortFilms", 6 },
                    { 71, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6335), "Documentary screenings", 3, "Documentaries", 6 },
                    { 72, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6336), "Animation film screenings", 4, "Animation", 6 },
                    { 73, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6337), "Independent cinema screenings", 5, "IndependentCinema", 6 },
                    { 74, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6338), "Bulgarian cinema screenings", 6, "BulgarianCinema", 6 },
                    { 75, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6340), "International cinema screenings", 7, "InternationalCinema", 6 },
                    { 76, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6341), "Film premiere events", 8, "FilmPremieres", 6 },
                    { 77, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6342), "Student film screenings", 9, "StudentFilms", 6 },
                    { 78, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6343), "Film festival events", 10, "FilmFestivals", 6 },
                    { 79, 6, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6344), "Other cinema events", 99, "Other", 6 },
                    { 80, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6350), "Music festival events", 1, "MusicFestivals", 7 },
                    { 81, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6352), "Film festival events", 2, "FilmFestivals", 7 },
                    { 82, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6354), "Art festival events", 3, "ArtFestivals", 7 },
                    { 83, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6355), "Food and wine festivals", 4, "FoodAndWineFestivals", 7 },
                    { 84, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6356), "Cultural festival events", 5, "CulturalFestivals", 7 },
                    { 85, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6357), "Folklore festival events", 6, "FolkloreFestivals", 7 },
                    { 86, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6358), "Street festival events", 7, "StreetFestivals", 7 },
                    { 87, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6360), "Summer festival events", 8, "SummerFestivals", 7 },
                    { 88, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6394), "Light festival events", 9, "LightFestivals", 7 },
                    { 89, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6396), "Craft beer festivals", 10, "CraftBeerFestivals", 7 },
                    { 90, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6397), "Eco festival events", 11, "EcoFestivals", 7 },
                    { 91, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6398), "Dance festival events", 12, "DanceFestivals", 7 },
                    { 92, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6399), "Tech festival events", 13, "TechFestivals", 7 },
                    { 93, 7, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6401), "Other festival events", 99, "Other", 7 },
                    { 94, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6409), "Art exhibitions", 1, "ArtExhibitions", 8 },
                    { 95, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6412), "Photography exhibitions", 2, "PhotographyExhibitions", 8 },
                    { 96, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6413), "Historical exhibitions", 3, "HistoricalExhibitions", 8 },
                    { 97, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6414), "Science exhibitions", 4, "ScienceExhibitions", 8 },
                    { 98, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6415), "Technology exhibitions", 5, "TechnologyExhibitions", 8 },
                    { 99, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6417), "Automotive exhibitions", 6, "AutomotiveExhibitions", 8 },
                    { 100, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6418), "Design exhibitions", 7, "DesignExhibitions", 8 },
                    { 101, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6419), "Cultural heritage exhibitions", 8, "CulturalHeritageExhibitions", 8 },
                    { 102, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6420), "Educational exhibitions", 9, "EducationalExhibitions", 8 },
                    { 103, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6421), "Craft exhibitions", 10, "CraftExhibitions", 8 },
                    { 104, 8, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6422), "Other exhibitions", 99, "Other", 8 },
                    { 105, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6427), "Technology conferences", 1, "TechConferences", 9 },
                    { 106, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6430), "Business conferences", 2, "BusinessConferences", 9 },
                    { 107, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6431), "Startup conferences", 3, "StartupConferences", 9 },
                    { 108, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6432), "Academic conferences", 4, "AcademicConferences", 9 },
                    { 109, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6433), "Marketing conferences", 5, "MarketingConferences", 9 },
                    { 110, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6434), "Science conferences", 6, "ScienceConferences", 9 },
                    { 111, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6436), "Health and medicine conferences", 7, "HealthAndMedicineConferences", 9 },
                    { 112, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6437), "AI and innovation conferences", 8, "AIAndInnovationConferences", 9 },
                    { 113, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6438), "IT security conferences", 9, "ITSecurityConferences", 9 },
                    { 114, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6439), "Environmental conferences", 10, "EnvironmentalConferences", 9 },
                    { 115, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6440), "HR conferences", 11, "HRConferences", 9 },
                    { 116, 9, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6441), "Other conferences", 99, "Other", 9 },
                    { 117, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6447), "Art workshops", 1, "ArtWorkshops", 10 },
                    { 118, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6450), "Music workshops", 2, "MusicWorkshops", 10 },
                    { 119, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6451), "Dance workshops", 3, "DanceWorkshops", 10 },
                    { 120, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6452), "Photography workshops", 4, "PhotographyWorkshops", 10 },
                    { 121, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6453), "Cooking workshops", 5, "CookingWorkshops", 10 },
                    { 122, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6455), "Craft workshops", 6, "CraftWorkshops", 10 },
                    { 123, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6456), "Startup and entrepreneurship workshops", 7, "StartupAndEntrepreneurshipWorkshops", 10 },
                    { 124, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6457), "Personal development workshops", 8, "PersonalDevelopmentWorkshops", 10 },
                    { 125, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6458), "Coding workshops", 9, "CodingWorkshops", 10 },
                    { 126, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6459), "Language workshops", 10, "LanguageWorkshops", 10 },
                    { 127, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6460), "Theatre workshops", 11, "TheatreWorkshops", 10 },
                    { 128, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6461), "Yoga and wellness workshops", 12, "YogaAndWellnessWorkshops", 10 },
                    { 129, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6462), "Marketing workshops", 13, "MarketingWorkshops", 10 },
                    { 130, 10, new DateTime(2025, 11, 28, 14, 42, 47, 943, DateTimeKind.Utc).AddTicks(6465), "Other workshops", 99, "Other", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SubCategories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

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
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2991));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2994));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2996));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(2997));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3031));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3035));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3039));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3040));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3042));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3044));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3045));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3049));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3053));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3054));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3057));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3064), "Football matches and events", 1, "Football", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3066), "Basketball games and tournaments", 2, "Basketball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3069), "Tennis matches and tournaments", 3, "Tennis", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3070), "Volleyball games", 4, "Volleyball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3072), "Swimming competitions", 5, "Swimming", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3073), "Track and field events", 6, "Athletics", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3079), "Boxing events and competitions", 7, "Boxing", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3082), "Wrestling events and competitions", 8, "Wrestling", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3085), "Gymnastics events and competitions", 9, "Gymnastics", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 28, 11, 47, 10, 181, DateTimeKind.Utc).AddTicks(3088), "Cycling events and competitions", 10, "Cycling", 4 });
        }
    }
}
