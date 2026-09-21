using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace HotelManagementSystem.Migrations
{
 [DbContext(typeof(HotelManagementSystem.Infrastructure.HmsDbContext))]
 [Migration("20260910200000_AddCarRentals")]
 public partial class AddCarRentals : Migration
 {
  protected override void Up(MigrationBuilder migrationBuilder)
  {
   migrationBuilder.CreateTable(name:"Cars",columns:table=>new{Id=table.Column<int>(nullable:false).Annotation("SqlServer:Identity","1, 1"),City=table.Column<string>(nullable:false),Brand=table.Column<string>(nullable:false),Model=table.Column<string>(nullable:false),Year=table.Column<int>(nullable:false),Category=table.Column<string>(nullable:false),DailyPrice=table.Column<decimal>(type:"decimal(18,2)",nullable:false),Seats=table.Column<int>(nullable:false),Luggage=table.Column<int>(nullable:false),Transmission=table.Column<string>(nullable:false),Fuel=table.Column<string>(nullable:false),AirConditioning=table.Column<bool>(nullable:false),UnlimitedKilometers=table.Column<bool>(nullable:false)},constraints:table=>table.PrimaryKey("PK_Cars",x=>x.Id));
   migrationBuilder.CreateTable(name:"CarRentals",columns:table=>new{Id=table.Column<int>(nullable:false).Annotation("SqlServer:Identity","1, 1"),CarId=table.Column<int>(nullable:false),GuestId=table.Column<int>(nullable:false),PickupDate=table.Column<DateOnly>(nullable:false),DropoffDate=table.Column<DateOnly>(nullable:false),DrivingLicenseId=table.Column<string>(nullable:false),PickupLocation=table.Column<string>(nullable:false)},constraints:table=>{table.PrimaryKey("PK_CarRentals",x=>x.Id);table.ForeignKey(name:"FK_CarRentals_Cars_CarId",column:x=>x.CarId,principalTable:"Cars",principalColumn:"Id",onDelete:ReferentialAction.Cascade);table.ForeignKey(name:"FK_CarRentals_Guests_GuestId",column:x=>x.GuestId,principalTable:"Guests",principalColumn:"Id",onDelete:ReferentialAction.Cascade);});
   migrationBuilder.CreateIndex(name:"IX_CarRentals_CarId",table:"CarRentals",column:"CarId");migrationBuilder.CreateIndex(name:"IX_CarRentals_GuestId",table:"CarRentals",column:"GuestId");
  }
  protected override void Down(MigrationBuilder migrationBuilder){migrationBuilder.DropTable(name:"CarRentals");migrationBuilder.DropTable(name:"Cars");}
 }
}
