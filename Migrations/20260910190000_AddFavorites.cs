using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace HotelManagementSystem.Migrations
{
    [DbContext(typeof(HotelManagementSystem.Infrastructure.HmsDbContext))]
    [Migration("20260910190000_AddFavorites")]
    public partial class AddFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Favorites",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                GuestId = table.Column<int>(type: "int", nullable: false),
                HotelId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Favorites", x => x.Id);
                table.ForeignKey(name: "FK_Favorites_Guests_GuestId", column: x => x.GuestId, principalTable: "Guests", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey(name: "FK_Favorites_Hotels_HotelId", column: x => x.HotelId, principalTable: "Hotels", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateIndex(name: "IX_Favorites_GuestId_HotelId", table: "Favorites", columns: new[] { "GuestId", "HotelId" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_Favorites_HotelId", table: "Favorites", column: "HotelId");
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Favorites");
    }
}
