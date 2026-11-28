using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddEventIntroBg",
                table: "SiteContents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddEventIntroEn",
                table: "SiteContents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Culture = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CanonicalVenueId = table.Column<int>(type: "int", nullable: true),
                    NewVenueName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NewVenueAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TicketUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    OrganizerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrganizerPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OrganizerWebsite = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConsentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSubmissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSubmissions_CanonicalVenues_CanonicalVenueId",
                        column: x => x.CanonicalVenueId,
                        principalTable: "CanonicalVenues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventSubmissions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventSubmissions_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventSubmissionActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PerformedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PerformedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubmissionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSubmissionActions_EventSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "EventSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSubmissionDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubmissionDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSubmissionDates_EventSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "EventSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSubmissionDates_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "SiteContents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddEventIntroBg", "AddEventIntroEn" },
                values: new object[] { "Организираш събитие в София? Изпрати го тук. Ще го прегледаме и публикуваме до 2 работни дни.", "Organising an event in Sofia? Submit it here. We will review it and publish it within 2 working days." });

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissionActions_SubmissionId",
                table: "EventSubmissionActions",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissionDates_EventId",
                table: "EventSubmissionDates",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissionDates_SubmissionId_Date",
                table: "EventSubmissionDates",
                columns: new[] { "SubmissionId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissions_CanonicalVenueId",
                table: "EventSubmissions",
                column: "CanonicalVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissions_CategoryId",
                table: "EventSubmissions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissions_Status_SubmittedAtUtc",
                table: "EventSubmissions",
                columns: new[] { "Status", "SubmittedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissions_SubCategoryId",
                table: "EventSubmissions",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubmissions_UserId_SubmittedAtUtc",
                table: "EventSubmissions",
                columns: new[] { "UserId", "SubmittedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventSubmissionActions");

            migrationBuilder.DropTable(
                name: "EventSubmissionDates");

            migrationBuilder.DropTable(
                name: "EventSubmissions");

            migrationBuilder.DropColumn(
                name: "AddEventIntroBg",
                table: "SiteContents");

            migrationBuilder.DropColumn(
                name: "AddEventIntroEn",
                table: "SiteContents");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Events");
        }
    }
}
