using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVenuesEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanonicalVenueId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanonicalVenues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonicalVenues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VenueAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AliasString = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NormalizedString = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CanonicalVenueId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueAliases_CanonicalVenues_CanonicalVenueId",
                        column: x => x.CanonicalVenueId,
                        principalTable: "CanonicalVenues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CanonicalVenueId",
                table: "Events",
                column: "CanonicalVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonicalVenues_Slug",
                table: "CanonicalVenues",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueAliases_CanonicalVenueId",
                table: "VenueAliases",
                column: "CanonicalVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAliases_NormalizedString",
                table: "VenueAliases",
                column: "NormalizedString");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_CanonicalVenues_CanonicalVenueId",
                table: "Events",
                column: "CanonicalVenueId",
                principalTable: "CanonicalVenues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_CanonicalVenues_CanonicalVenueId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "VenueAliases");

            migrationBuilder.DropTable(
                name: "CanonicalVenues");

            migrationBuilder.DropIndex(
                name: "IX_Events_CanonicalVenueId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CanonicalVenueId",
                table: "Events");
        }
    }
}
