using System.Security.Claims;
using HotelManagementSystem.Application;
using HotelManagementSystem.Contracts;
using HotelManagementSystem.Domain;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HotelManagementSystem.Controllers;
[ApiController, Route("api/reservations")]
public class ReservationsController(HmsDbContext db,ReservationService service):ControllerBase
{
 [Authorize, HttpGet] public async Task<ActionResult<ApiResponse<object>>> Search(int? hotelId,int? guestId,int? roomId,DateOnly? date,bool? active){var q=db.Reservations.AsQueryable();if(User.IsInRole("Guest"))q=q.Where(x=>x.GuestId==int.Parse(User.FindFirstValue("guestId")!));else if(guestId.HasValue)q=q.Where(x=>x.GuestId==guestId);if(hotelId.HasValue)q=q.Where(x=>x.ReservationRooms.Any(y=>y.Room.HotelId==hotelId));if(roomId.HasValue)q=q.Where(x=>x.ReservationRooms.Any(y=>y.RoomId==roomId));if(date.HasValue)q=q.Where(x=>x.CheckInDate<=date&&x.CheckOutDate>date);if(active.HasValue){var today=DateOnly.FromDateTime(DateTime.UtcNow);q=active.Value?q.Where(x=>x.CheckOutDate>=today):q.Where(x=>x.CheckOutDate<today);}var result=await q.OrderByDescending(x=>x.CheckInDate).Select(x=>new{x.Id,x.CheckInDate,x.CheckOutDate,ReservationRooms=x.ReservationRooms.Select(y=>new{Room=new{y.Room.Id,y.Room.Name,y.Room.Price,y.Room.Capacity}})}).ToListAsync();return Ok(new ApiResponse<object>(true,result));}
 [Authorize, HttpPut("{id:int}")] public async Task<ActionResult<ApiResponse<Reservation>>> Update(int id,ReservationDatesRequest r){var res=await db.Reservations.FindAsync(id)??throw new DomainException("Reservation not found.");var g=User.FindFirstValue("guestId");if(!User.IsInRole("Admin")&&g!=res.GuestId.ToString())return Forbid();await service.UpdateDates(res,r);return Ok(new ApiResponse<Reservation>(true,res));}
 [Authorize, HttpDelete("{id:int}")] public async Task<ActionResult<ApiResponse<object>>> Cancel(int id){var res=await db.Reservations.FindAsync(id)??throw new DomainException("Reservation not found.");var g=User.FindFirstValue("guestId");if(!User.IsInRole("Admin")&&g!=res.GuestId.ToString())return Forbid();var fee=await service.Cancel(res);return Ok(new ApiResponse<object>(true,new{fee},"Reservation cancelled and confirmation email sent."));}
}
