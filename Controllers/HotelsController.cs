using HotelManagementSystem.Contracts;
using HotelManagementSystem.Domain;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HotelManagementSystem.Controllers;
[ApiController, Route("api/hotels")]
public class HotelsController(HmsDbContext db) : ControllerBase
{
 [ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
 [HttpGet] public async Task<ActionResult<ApiResponse<object>>> Get(string? country,string? city,int? rating) { var q=db.Hotels.AsNoTracking().AsQueryable(); if(!string.IsNullOrWhiteSpace(country))q=q.Where(x=>x.Country==country);if(!string.IsNullOrWhiteSpace(city))q=q.Where(x=>x.City==city);if(rating.HasValue)q=q.Where(x=>x.Rating==rating);return Ok(new ApiResponse<object>(true,await q.ToListAsync())); }
 [HttpGet("{hotelId:int}")] public async Task<ActionResult<ApiResponse<object>>> GetById(int hotelId) {var h=await db.Hotels.Where(x=>x.Id==hotelId).Select(x=>new{x.Id,x.Name,x.Rating,x.Country,x.City,x.Address}).SingleOrDefaultAsync();return h is null?NotFound(new ApiResponse<object>(false,null,"Hotel not found.")):Ok(new ApiResponse<object>(true,h));}
 [Authorize(Roles="Admin"),HttpPost] public async Task<ActionResult<ApiResponse<Hotel>>> Create(HotelRequest r) { Validate(r);var h=new Hotel{Name=r.Name,Rating=r.Rating,Country=r.Country,City=r.City,Address=r.Address};db.Hotels.Add(h);await db.SaveChangesAsync();return CreatedAtAction(nameof(GetById),new{hotelId=h.Id},new ApiResponse<Hotel>(true,h)); }
 [Authorize(Roles="Admin,Manager"),HttpPut("{hotelId:int}")] public async Task<ActionResult<ApiResponse<Hotel>>> Update(int hotelId,HotelRequest r) { Validate(r);var h=await db.Hotels.FindAsync(hotelId)??throw new HotelManagementSystem.Application.DomainException("Hotel not found.");h.Name=r.Name;h.Rating=r.Rating;h.Country=r.Country;h.City=r.City;h.Address=r.Address;await db.SaveChangesAsync();return Ok(new ApiResponse<Hotel>(true,h));}
 [Authorize(Roles="Admin"),HttpDelete("{hotelId:int}")] public async Task<IActionResult> Delete(int hotelId) {var h=await db.Hotels.FindAsync(hotelId)??throw new HotelManagementSystem.Application.DomainException("Hotel not found.");if(await db.Rooms.AnyAsync(x=>x.HotelId==hotelId)||await db.ReservationRooms.AnyAsync(x=>x.Room.HotelId==hotelId&&x.Reservation.CheckOutDate>=DateOnly.FromDateTime(DateTime.UtcNow)))throw new HotelManagementSystem.Application.DomainException("A hotel with rooms or active reservations cannot be deleted.");db.Hotels.Remove(h);await db.SaveChangesAsync();return NoContent();}
 static void Validate(HotelRequest r){if(string.IsNullOrWhiteSpace(r.Name)||r.Rating is <1 or >5)throw new HotelManagementSystem.Application.DomainException("Name is required and rating must be between 1 and 5.");}
}
