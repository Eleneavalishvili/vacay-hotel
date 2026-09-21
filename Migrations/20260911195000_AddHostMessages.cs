using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Migrations
{
    [DbContext(typeof(HotelManagementSystem.Infrastructure.HmsDbContext))]
    [Migration("20260911195000_AddHostMessages")]
    public partial class AddHostMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HostMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(nullable: false),
                    GuestId = table.Column<int>(nullable: false),
                    Sender = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    SentAtUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostMessages", x => x.Id);
                    table.ForeignKey(name: "FK_HostMessages_Reservations_ReservationId", column: x => x.ReservationId, principalTable: "Reservations", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(name: "FK_HostMessages_Guests_GuestId", column: x => x.GuestId, principalTable: "Guests", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
                });
            migrationBuilder.CreateIndex(name: "IX_HostMessages_ReservationId", table: "HostMessages", column: "ReservationId");
            migrationBuilder.CreateIndex(name: "IX_HostMessages_GuestId", table: "HostMessages", column: "GuestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "HostMessages");
    }
}
