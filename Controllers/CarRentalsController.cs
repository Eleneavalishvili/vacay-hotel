using System.Security.Claims;
using HotelManagementSystem.Application;
using HotelManagementSystem.Contracts;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers;

[ApiController, Route("api/car-rentals")]
public class CarRentalsController(HmsDbContext db, CarRentalService rentals) : ControllerBase
{
 [HttpGet("cars")] public async Task<ActionResult<ApiResponse<object>>> Cars(string? city,DateOnly? pickupDate,DateOnly? dropoffDate)
 {
  var q=db.Cars.AsQueryable(); if(!string.IsNullOrWhiteSpace(city))q=q.Where(x=>x.City==city);
  if(pickupDate.HasValue&&dropoffDate.HasValue){var busy=db.CarRentals.Where(x=>x.PickupDate<dropoffDate&&x.DropoffDate>pickupDate).Select(x=>x.CarId);q=q.Where(x=>!busy.Contains(x.Id));}
  var result=await q.OrderBy(x=>x.DailyPrice).Select(x=>new{x.Id,x.City,x.Brand,x.Model,x.Year,x.Category,x.DailyPrice,x.Seats,x.Luggage,x.Transmission,x.Fuel,x.AirConditioning,x.UnlimitedKilometers}).ToListAsync();
  return Ok(new ApiResponse<object>(true,result));
 }
 [Authorize(Roles="Guest"),HttpPost] public async Task<ActionResult<ApiResponse<object>>> Create(CarRentalRequest request)
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);var rental=await rentals.Create(guestId,request);return Ok(new ApiResponse<object>(true,new{rental.Id,rental.PickupDate,rental.DropoffDate}));
 }
 [Authorize(Roles="Guest"),HttpGet("mine")] public async Task<ActionResult<ApiResponse<object>>> Mine()
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);var result=await db.CarRentals.Where(x=>x.GuestId==guestId).Include(x=>x.Car).OrderByDescending(x=>x.PickupDate).Select(x=>new{x.Id,x.PickupDate,x.DropoffDate,x.PickupLocation,Car=new{x.Car.Brand,x.Car.Model,x.Car.City,x.Car.Year,x.Car.DailyPrice}}).ToListAsync();return Ok(new ApiResponse<object>(true,result));
 }
 [Authorize(Roles="Guest"),HttpDelete("{id:int}")] public async Task<ActionResult<ApiResponse<object>>> Cancel(int id)
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);var rental=await db.CarRentals.SingleOrDefaultAsync(x=>x.Id==id&&x.GuestId==guestId)??throw new DomainException("Car rental not found.");var fee=await rentals.Cancel(rental);return Ok(new ApiResponse<object>(true,new{fee},"Car rental cancelled and confirmation email sent."));
 }
}
