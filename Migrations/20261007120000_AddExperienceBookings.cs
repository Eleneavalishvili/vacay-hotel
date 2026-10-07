using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Migrations;

[DbContext(typeof(HotelManagementSystem.Infrastructure.HmsDbContext))]
[Migration("20261007120000_AddExperienceBookings")]
public partial class AddExperienceBookings : Migration
{
 protected override void Up(MigrationBuilder migrationBuilder)
 {
  migrationBuilder.CreateTable(
   name: "ExperienceBookings",
   columns: table => new
   {
    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
    GuestId = table.Column<int>(nullable: false),
    City = table.Column<string>(nullable: false),
    Title = table.Column<string>(nullable: false),
    Category = table.Column<string>(nullable: false),
    Address = table.Column<string>(nullable: false),
    Date = table.Column<DateOnly>(type: "date", nullable: false),
    ArrivalTime = table.Column<TimeOnly>(type: "time", nullable: false),
    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
    Participants = table.Column<int>(nullable: false)
   },
   constraints: table =>
   {
    table.PrimaryKey("PK_ExperienceBookings", x => x.Id);
    table.ForeignKey(name: "FK_ExperienceBookings_Guests_GuestId", column: x => x.GuestId, principalTable: "Guests", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
   });
  migrationBuilder.CreateIndex(name: "IX_ExperienceBookings_GuestId", table: "ExperienceBookings", column: "GuestId");
 }

 protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "ExperienceBookings");
}
