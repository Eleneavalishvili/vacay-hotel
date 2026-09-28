using HotelManagementSystem.Contracts;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(HmsDbContext db) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<object>>> Dashboard()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var reservations = await db.Reservations
            .Include(x => x.Guest)
            .Include(x => x.ReservationRooms).ThenInclude(x => x.Room).ThenInclude(x => x.Hotel)
            .OrderByDescending(x => x.CheckInDate)
            .Take(30)
            .ToListAsync();
        var rentals = await db.CarRentals
            .Include(x => x.Guest)
            .Include(x => x.Car)
            .OrderByDescending(x => x.PickupDate)
            .Take(30)
            .ToListAsync();
        var managers = await db.Managers
            .Include(x => x.Hotel)
            .OrderBy(x => x.Hotel.Name).ThenBy(x => x.LastName)
            .Select(x => new { x.Id, x.FirstName, x.LastName, x.Email, x.PhoneNumber, Hotel = x.Hotel.Name, x.HotelId })
            .ToListAsync();

        return Ok(new ApiResponse<object>(true, new
        {
            totalHotels = await db.Hotels.CountAsync(),
            totalRooms = await db.Rooms.CountAsync(),
            totalGuests = await db.Guests.CountAsync(),
            activeReservations = await db.Reservations.CountAsync(x => x.CheckOutDate >= today),
            totalCarRentals = await db.CarRentals.CountAsync(),
            reservations = reservations.Select(x => new
            {
                x.Id,
                x.CheckInDate,
                x.CheckOutDate,
                guest = new { x.Guest.FirstName, x.Guest.LastName },
                rooms = x.ReservationRooms.Select(y => new { y.Room.Id, y.Room.Name, y.Room.Price, hotel = y.Room.Hotel.Name })
            }),
            rentals = rentals.Select(x => new
            {
                x.Id,
                x.PickupDate,
                x.DropoffDate,
                x.PickupLocation,
                guest = new { x.Guest.FirstName, x.Guest.LastName },
                car = new { x.Car.Brand, x.Car.Model, x.Car.City, x.Car.DailyPrice }
            }),
            managers
        }));
    }
}
