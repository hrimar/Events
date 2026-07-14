using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteContentAndSeoMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageSeoMetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TitleBg = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionBg = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSeoMetas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroTitleBg = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroTitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroSubtitleBg = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HeroSubtitleEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AboutUsContentBg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutUsContentEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContents", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PageSeoMetas",
                columns: new[] { "Id", "DescriptionBg", "DescriptionEn", "PageKey", "TitleBg", "TitleEn", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, null, "home", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, null, null, "about-us", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, null, null, "category-music", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, null, null, "category-art", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, null, null, "category-business", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, null, null, "category-sports", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, null, null, "category-theatre", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, null, null, "category-cinema", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, null, null, "category-festivals", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, null, null, "category-exhibitions", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, null, null, "category-conferences", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, null, null, "category-workshops", null, null, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SiteContents",
                columns: new[] { "Id", "AboutUsContentBg", "AboutUsContentEn", "HeroSubtitleBg", "HeroSubtitleEn", "HeroTitleBg", "HeroTitleEn", "UpdatedAt" },
                values: new object[] { 1, "<p>Създадохме Go Sofia с една ясна цел – да направим откриването на събития в София по-лесно, по-удобно и по-достъпно. В динамичен град като София всеки ден се случват концерти, фестивали, изложби, спектакли, семейни събития и още много интересни инициативи, но тази информация често е разпръсната на различни места и трудно достига до хората навреме.</p><p>С go-sofia.com искаме да съберем на едно място информация за събитията в града, така че жителите на София и посетителите на града да могат по-лесно да откриват какво се случва около тях. Нашата идея е да помогнем на повече хора да намират събития, които отговарят на интересите им, и да планират по-лесно свободното си време в София.</p><p>Вярваме, че когато достъпът до такава информация е по-лесен, градът става по-жив, по-свързан и по-интересен както за хората, които живеят тук, така и за тези, които го посещават.</p>", "<p>We created Go Sofia with a simple goal: to make discovering events in Sofia easier, more convenient, and more accessible. In a city as active and diverse as Sofia, there are concerts, festivals, exhibitions, performances, family events, and many other activities happening all the time. Yet this information is often scattered across different websites, social media pages, and local sources, making it difficult to find in one place.</p><p>With go-sofia.com, we aim to bring together useful and up-to-date event information so that both Sofia residents and international visitors can more easily explore what is happening in the city. Our goal is to help people discover events that match their interests and make it easier to enjoy Sofia's cultural and urban life.</p><p>We believe that when event information is easier to access, the city becomes more open, more connected, and more enjoyable for everyone — whether you live here or are visiting Sofia for a few days.</p>", "Концерти, театър, спорт и много повече. Намерете следващото си незабравимо преживяване!", "Concerts, theatre, sports, and much more. Find your next amazing experience!", "Открийте най-добрите събития в София", "Discover the Best Events in Sofia", new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_PageSeoMetas_PageKey",
                table: "PageSeoMetas",
                column: "PageKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageSeoMetas");

            migrationBuilder.DropTable(
                name: "SiteContents");
        }
    }
}
