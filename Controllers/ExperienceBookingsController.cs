using System.Security.Claims;
using HotelManagementSystem.Application;
using HotelManagementSystem.Contracts;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers;

[ApiController, Route("api/experience-bookings")]
public class ExperienceBookingsController(HmsDbContext db, ExperienceBookingService experiences) : ControllerBase
{
 [Authorize(Roles="Guest"), HttpPost]
 public async Task<ActionResult<ApiResponse<object>>> Create(ExperienceBookingRequest request)
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);
  var booking=await experiences.Create(guestId,request);
  return Ok(new ApiResponse<object>(true,new {booking.Id,booking.City,booking.Title,booking.Address,booking.Date,booking.ArrivalTime,booking.Price,booking.Participants},"Experience booking confirmed."));
 }

 [Authorize(Roles="Guest"), HttpGet("mine")]
 public async Task<ActionResult<ApiResponse<object>>> Mine()
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);
  var result=await db.ExperienceBookings.Where(x=>x.GuestId==guestId).OrderByDescending(x=>x.Date).ThenBy(x=>x.ArrivalTime).Select(x=>new {x.Id,x.City,x.Title,x.Category,x.Address,x.Date,x.ArrivalTime,x.Price,x.Participants}).ToListAsync();
  return Ok(new ApiResponse<object>(true,result));
 }

 [Authorize(Roles="Guest"), HttpDelete("{id:int}")]
 public async Task<ActionResult<ApiResponse<object>>> Cancel(int id)
 {
  var guestId=int.Parse(User.FindFirstValue("guestId")!);
  var booking=await db.ExperienceBookings.SingleOrDefaultAsync(x=>x.Id==id&&x.GuestId==guestId)??throw new DomainException("Experience booking not found.");
  var result=await experiences.Cancel(booking);
  var message=result.EmailSent?"Experience booking cancelled and confirmation email sent.":"Experience booking cancelled successfully. Email delivery is temporarily unavailable.";
  return Ok(new ApiResponse<object>(true,new {fee=result.Fee,emailSent=result.EmailSent},message));
 }
}
