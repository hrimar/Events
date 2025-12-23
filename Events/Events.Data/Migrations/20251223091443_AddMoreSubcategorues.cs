using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSubcategorues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(262));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(270));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(272));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(274));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(276));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(277));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(279));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(281));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(282));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(284));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(285));

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(658), "Pop music events and concerts", "Pop" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(674), "Rock music events and concerts", "Rock" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(676), "Hip-hop music events and concerts", "HipHop" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(678), "Rap music events and concerts", "Rap" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(680), "Jazz performances and sessions", "Jazz" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(684), "Blues music performances", "Blues" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(685), "Classical music concerts", "Classical" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(687), "Folk and traditional music", "Folk" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(688), "Traditional Bulgarian folklore music", "TraditionalBulgarian" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(691), "Electronic Dance Music events", "EDM" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(693), "Techno music events", "Techno" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(695), "House music events", "House" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(696), "Drum & Bass music events", "DrumBass" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(698), "Trance music events", "Trance" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(699), "Reggae music events", "Reggae" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(701), "R&B music events", 16, "RB" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(703), "Metal music events and concerts", 17, "Metal", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(706), "Indie music events", 18, "Indie", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(707), "Acoustic music events", 19, "Acoustic", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(709), "Alternative music events", 20, "Alternative", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(710), "Punk music events", 21, "Punk", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(712), "Soul music events", 22, "Soul", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(714), "Chillout music events", 23, "Chillout", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(715), "Experimental music events", 24, "Experimental", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(717), "Choir performances", 25, "Choir", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(718), "World music events", 26, "WorldMusic", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(720), "Other music events", 99, "Other", 1 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(727), "Painting exhibitions and shows", 1, "Painting" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(731), "Sculpture exhibitions", 2, "Sculpture" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(733), "Photography exhibitions", 3, "Photography" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(734), "Digital art exhibitions", 4, "DigitalArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(736), "Street art and urban art events", 5, "StreetArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(738), "Graffiti art exhibitions", 6, "Graffiti", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(830), "Illustration exhibitions", 7, "Illustration", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(832), "Performance art events", 8, "PerformanceArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(834), "Installation art exhibitions", 9, "InstallationArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(836), "Contemporary art exhibitions", 10, "ContemporaryArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(837), "Visual arts events", 11, "VisualArts", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(839), "Mixed media art exhibitions", 12, "MixedMedia", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(841), "Conceptual art exhibitions", 13, "ConceptualArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(843), "Other art events", 99, "Other", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(851), "Business networking events", 1, "NetworkingEvents" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(854), "Startup events and pitches", 2, "Startups" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(856), "Entrepreneurship seminars", 3, "Entrepreneurship" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(857), "Marketing conferences", 4, "Marketing" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(859), "Sales training and events", 5, "Sales" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(861), "Leadership development events", 6, "Leadership", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(862), "Finance and investment events", 7, "Finance", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(864), "Real estate events", 8, "RealEstate", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(866), "Investment seminars", 9, "Investment", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(867), "E-commerce events", 10, "ECommerce", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(869), "Innovation forums", 11, "Innovation", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(871), "Technology business events", 12, "Technology", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(873), "HR and management events", 13, "HRManagement", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(875), "Business strategy workshops", 14, "BusinessStrategy", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(876), "Product development events", 15, "ProductDevelopment", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(878), "Other business events", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(884), "Football matches and events", "Football", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(889), "Basketball games and tournaments", "Basketball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(890), "Volleyball games and tournaments", "Volleyball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(892), "Tennis matches and tournaments", "Tennis", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(894), "Athletics and track & field events", "Athletics", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(896), "Swimming competitions", "Swimming", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(898), "Running events and marathons", "Running", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(899), "Cycling races and events", "Cycling", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(903), "Boxing matches", "Boxing", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(905), "Mixed Martial Arts events", "MMA", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(906), "Wrestling competitions", 11, "Wrestling", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(908), "Weightlifting competitions", 12, "Weightlifting", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(910), "CrossFit competitions", 13, "CrossFit", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(911), "Yoga events and classes", 14, "Yoga", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(913), "Fitness events", 15, "Fitness", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(915), "Hiking and trekking events", 16, "Hiking", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(916), "Climbing competitions and events", 17, "Climbing", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(918), "Skiing competitions", 18, "Skiing", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(920), "Snowboarding competitions", 19, "Snowboarding", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(922), "Motocross races", 20, "Motocross", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(923), "eSports tournaments", 21, "eSports", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(925), "Table tennis tournaments", 22, "TableTennis", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(927), "Badminton tournaments", 23, "Badminton", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(929), "Golf tournaments", 24, "Golf", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(930), "Dance sport competitions", 25, "DanceSport", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(932), "Other sports events and competitions", 99, "Other", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(979), "Drama theatre performances", 1, "Drama", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(983), "Comedy theatre shows", 2, "Comedy", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(985), "Musical theatre productions", 3, "MusicalTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(986), "Tragedy theatre performances", 4, "Tragedy", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(988), "Experimental theatre", 5, "ExperimentalTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(990), "Puppet theatre shows", 6, "PuppetTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(992), "Improvisation theatre", 7, "Improvisation", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(993), "Street theatre performances", 8, "StreetTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(995), "Monodrama performances", 9, "Monodrama", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(996), "Children's theatre shows", 10, "ChildrensTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(998), "Stand-up comedy shows", 11, "StandupComedy", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1000), "Other theatre performances", 99, "Other", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1006), "Feature film screenings", 1, "FeatureFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1011), "Short film screenings", 2, "ShortFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1013), "Documentary screenings", 3, "Documentaries", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1015), "Animation film screenings", 4, "Animation", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1017), "Independent cinema screenings", 5, "IndependentCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1018), "Bulgarian cinema screenings", 6, "BulgarianCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1020), "International cinema screenings", 7, "InternationalCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1022), "Film premiere events", 8, "FilmPremieres", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1023), "Student film screenings", 9, "StudentFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1025), "Film festival events", 10, "FilmFestivals", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1027), "Other cinema events", 99, "Other", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1033), "Music festival events", 1, "MusicFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1036), "Film festival events", 2, "FilmFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1038), "Art festival events", 3, "ArtFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1040), "Food and wine festivals", 4, "FoodAndWineFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1041), "Cultural festival events", 5, "CulturalFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1043), "Folklore festival events", 6, "FolkloreFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1045), "Street festival events", 7, "StreetFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1047), "Summer festival events", 8, "SummerFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1048), "Light festival events", 9, "LightFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1050), "Craft beer festivals", 10, "CraftBeerFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1052), "Eco festival events", 11, "EcoFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1054), "Dance festival events", 12, "DanceFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1056), "Tech festival events", 13, "TechFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1057), "Other festival events", 99, "Other", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1063), "Art exhibitions", 1, "ArtExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1067), "Photography exhibitions", 2, "PhotographyExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1068), "Historical exhibitions", 3, "HistoricalExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1070), "Science exhibitions", 4, "ScienceExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1071), "Technology exhibitions", 5, "TechnologyExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1074), "Automotive exhibitions", 6, "AutomotiveExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1076), "Design exhibitions", 7, "DesignExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1077), "Cultural heritage exhibitions", 8, "CulturalHeritageExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1079), "Educational exhibitions", 9, "EducationalExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1122), "Craft exhibitions", 10, "CraftExhibitions", 8 });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[,]
                {
                    { 131, 8, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1124), "Other exhibitions", 99, "Other", 8 },
                    { 132, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1131), "Technology conferences", 1, "TechConferences", 9 },
                    { 133, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1134), "Business conferences", 2, "BusinessConferences", 9 },
                    { 134, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1136), "Startup conferences", 3, "StartupConferences", 9 },
                    { 135, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1137), "Academic conferences", 4, "AcademicConferences", 9 },
                    { 136, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1139), "Marketing conferences", 5, "MarketingConferences", 9 },
                    { 137, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1141), "Science conferences", 6, "ScienceConferences", 9 },
                    { 138, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1143), "Health and medicine conferences", 7, "HealthAndMedicineConferences", 9 },
                    { 139, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1145), "AI and innovation conferences", 8, "AIAndInnovationConferences", 9 },
                    { 140, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1146), "IT security conferences", 9, "ITSecurityConferences", 9 },
                    { 141, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1148), "Environmental conferences", 10, "EnvironmentalConferences", 9 },
                    { 142, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1150), "HR conferences", 11, "HRConferences", 9 },
                    { 143, 9, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1152), "Other conferences", 99, "Other", 9 },
                    { 144, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1158), "Art workshops", 1, "ArtWorkshops", 10 },
                    { 145, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1162), "Music workshops", 2, "MusicWorkshops", 10 },
                    { 146, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1163), "Dance workshops", 3, "DanceWorkshops", 10 },
                    { 147, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1165), "Photography workshops", 4, "PhotographyWorkshops", 10 },
                    { 148, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1167), "Cooking workshops", 5, "CookingWorkshops", 10 },
                    { 149, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1168), "Craft workshops", 6, "CraftWorkshops", 10 },
                    { 150, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1170), "Startup and entrepreneurship workshops", 7, "StartupAndEntrepreneurshipWorkshops", 10 },
                    { 151, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1172), "Personal development workshops", 8, "PersonalDevelopmentWorkshops", 10 },
                    { 152, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1174), "Coding workshops", 9, "CodingWorkshops", 10 },
                    { 153, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1175), "Language workshops", 10, "LanguageWorkshops", 10 },
                    { 154, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1177), "Theatre workshops", 11, "TheatreWorkshops", 10 },
                    { 155, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1179), "Yoga and wellness workshops", 12, "YogaAndWellnessWorkshops", 10 },
                    { 156, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1181), "Marketing workshops", 13, "MarketingWorkshops", 10 },
                    { 157, 10, new DateTime(2025, 12, 23, 9, 14, 41, 88, DateTimeKind.Utc).AddTicks(1182), "Other workshops", 99, "Other", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 157);

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
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6100), "Rock music events and concerts", "Rock" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6115), "Jazz performances and sessions", "Jazz" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6117), "Heavy metal and metal subgenres", "Metal" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6118), "Pop music events", "Pop" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6120), "Funk music events", "Funk" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6152), "Punk music events", "Punk" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6211), "Opera music events", "Opera" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6214), "Classical music concerts", "Classical" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6215), "Electronic music and DJ sets", "Electronic" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6218), "Folk and traditional music", "Folk" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6219), "Blues music performances", "Blues" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6220), "Country music events", "Country" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6223), "Reggae music events", "Reggae" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6225), "Hip-hop and rap events", "HipHop" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6227), "Alternative music events", "Alternative" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6229), "Other music events", 99, "Other" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6235), "Painting exhibitions and shows", 1, "Painting", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6238), "Sculpture exhibitions", 2, "Sculpture", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6239), "Photography exhibitions", 3, "Photography", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6240), "Digital art exhibitions", 4, "DigitalArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6242), "Street art and urban art events", 5, "StreetArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6243), "Graffiti art exhibitions", 6, "Graffiti", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6244), "Illustration exhibitions", 7, "Illustration", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6245), "Performance art events", 8, "PerformanceArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6246), "Installation art exhibitions", 9, "InstallationArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6248), "Contemporary art exhibitions", 10, "ContemporaryArt", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 2, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6249), "Visual arts events", 11, "VisualArts", 2 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6250), "Mixed media art exhibitions", 12, "MixedMedia" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6252), "Conceptual art exhibitions", 13, "ConceptualArt" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6253), "Other art events", 99, "Other" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6259), "Business networking events", 1, "NetworkingEvents", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6262), "Startup events and pitches", 2, "Startups", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6263), "Entrepreneurship seminars", 3, "Entrepreneurship", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6265), "Marketing conferences", 4, "Marketing", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6266), "Sales training and events", 5, "Sales", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6267), "Leadership development events", 6, "Leadership", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6269), "Finance and investment events", 7, "Finance", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6270), "Real estate events", 8, "RealEstate", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6271), "Investment seminars", 9, "Investment", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6272), "E-commerce events", 10, "ECommerce", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 3, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6273), "Innovation forums", 11, "Innovation", 3 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6274), "Technology business events", 12, "Technology" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6276), "HR and management events", 13, "HRManagement" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6277), "Business strategy workshops", 14, "BusinessStrategy" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6278), "Product development events", 15, "ProductDevelopment" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CreatedAt", "Description", "EnumValue", "Name" },
                values: new object[] { new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6279), "Other business events", 99, "Other" });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6286), "Football matches and events", 1, "Football", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6289), "Basketball games and tournaments", 2, "Basketball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6290), "Tennis matches and tournaments", 3, "Tennis", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6291), "Volleyball games", 4, "Volleyball", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6292), "Swimming competitions", 5, "Swimming", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6294), "Track and field events", 6, "Athletics", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6295), "Boxing events and competitions", 7, "Boxing", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6300), "Wrestling events and competitions", 8, "Wrestling", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6303), "Gymnastics events and competitions", 9, "Gymnastics", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6306), "Cycling events and competitions", 10, "Cycling", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "ParentCategory" },
                values: new object[] { 4, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6340), "Other sports events and competitions", 4 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6345), "Drama theatre performances", "Drama", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6347), "Comedy theatre shows", "Comedy", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6348), "Musical theatre productions", "MusicalTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6349), "Tragedy theatre performances", "Tragedy", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6351), "Experimental theatre", "ExperimentalTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6352), "Puppet theatre shows", "PuppetTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6353), "Improvisation theatre", "Improvisation", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6354), "Street theatre performances", "StreetTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6357), "Monodrama performances", "Monodrama", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6358), "Children's theatre shows", "ChildrensTheatre", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 5, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6359), "Other theatre performances", 99, "Other", 5 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6366), "Feature film screenings", 1, "FeatureFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6369), "Short film screenings", 2, "ShortFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6370), "Documentary screenings", 3, "Documentaries", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6371), "Animation film screenings", 4, "Animation", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6373), "Independent cinema screenings", 5, "IndependentCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6374), "Bulgarian cinema screenings", 6, "BulgarianCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6375), "International cinema screenings", 7, "InternationalCinema", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6377), "Film premiere events", 8, "FilmPremieres", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6378), "Student film screenings", 9, "StudentFilms", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6379), "Film festival events", 10, "FilmFestivals", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 6, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6380), "Other cinema events", 99, "Other", 6 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6384), "Music festival events", 1, "MusicFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6386), "Film festival events", 2, "FilmFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6387), "Art festival events", 3, "ArtFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6389), "Food and wine festivals", 4, "FoodAndWineFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6390), "Cultural festival events", 5, "CulturalFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6391), "Folklore festival events", 6, "FolkloreFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6392), "Street festival events", 7, "StreetFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6393), "Summer festival events", 8, "SummerFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6395), "Light festival events", 9, "LightFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6396), "Craft beer festivals", 10, "CraftBeerFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6397), "Eco festival events", 11, "EcoFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6398), "Dance festival events", 12, "DanceFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6399), "Tech festival events", 13, "TechFestivals", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 7, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6401), "Other festival events", 99, "Other", 7 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6405), "Art exhibitions", 1, "ArtExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6407), "Photography exhibitions", 2, "PhotographyExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6409), "Historical exhibitions", 3, "HistoricalExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6410), "Science exhibitions", 4, "ScienceExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6411), "Technology exhibitions", 5, "TechnologyExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6412), "Automotive exhibitions", 6, "AutomotiveExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6413), "Design exhibitions", 7, "DesignExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6415), "Cultural heritage exhibitions", 8, "CulturalHeritageExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6416), "Educational exhibitions", 9, "EducationalExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6417), "Craft exhibitions", 10, "CraftExhibitions", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 8, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6418), "Other exhibitions", 99, "Other", 8 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6423), "Technology conferences", 1, "TechConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6425), "Business conferences", 2, "BusinessConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6426), "Startup conferences", 3, "StartupConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6427), "Academic conferences", 4, "AcademicConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6459), "Marketing conferences", 5, "MarketingConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6460), "Science conferences", 6, "ScienceConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6462), "Health and medicine conferences", 7, "HealthAndMedicineConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6463), "AI and innovation conferences", 8, "AIAndInnovationConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6464), "IT security conferences", 9, "ITSecurityConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6465), "Environmental conferences", 10, "EnvironmentalConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6466), "HR conferences", 11, "HRConferences", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 9, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6467), "Other conferences", 99, "Other", 9 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6473), "Art workshops", 1, "ArtWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6476), "Music workshops", 2, "MusicWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6477), "Dance workshops", 3, "DanceWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6478), "Photography workshops", 4, "PhotographyWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6479), "Cooking workshops", 5, "CookingWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6480), "Craft workshops", 6, "CraftWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6482), "Startup and entrepreneurship workshops", 7, "StartupAndEntrepreneurshipWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6483), "Personal development workshops", 8, "PersonalDevelopmentWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6484), "Coding workshops", 9, "CodingWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6485), "Language workshops", 10, "LanguageWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6486), "Theatre workshops", 11, "TheatreWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6487), "Yoga and wellness workshops", 12, "YogaAndWellnessWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6489), "Marketing workshops", 13, "MarketingWorkshops", 10 });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "EnumValue", "Name", "ParentCategory" },
                values: new object[] { 10, new DateTime(2025, 11, 30, 14, 19, 8, 147, DateTimeKind.Utc).AddTicks(6491), "Other workshops", 99, "Other", 10 });
        }
    }
}
