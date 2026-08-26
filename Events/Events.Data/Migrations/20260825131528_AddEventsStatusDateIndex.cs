using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsStatusDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Offline index build (ONLINE = ON is not supported on Azure SQL Basic tier).
            // Apply outside peak traffic hours - briefly locks the Events table while building.
            migrationBuilder.CreateIndex(
                name: "IX_Events_Status_Date",
                table: "Events",
                columns: new[] { "Status", "Date" })
                .Annotation("SqlServer:Include", new[] { "CanonicalVenueId", "CategoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_Status_Date",
                table: "Events");
        }
    }
}
